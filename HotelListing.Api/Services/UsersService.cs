using HotelListing.Api.Constants;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Auth;
using HotelListing.Api.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelListing.Api.Services;

public class UsersService(UserManager<ApplicationUser> userManager, IConfiguration configuration) : IUsersService
{
    // Register
    public async Task<Result<RegisteredUserDto>> RegisterAsync(RegisterUserDto registerUserDto)
    {
        var user = new ApplicationUser
        {
            Email = registerUserDto.Email,
            FirstName = registerUserDto.FirstName,
            LastName = registerUserDto.LastName,
            UserName = registerUserDto.Email
        };
        var result = await userManager.CreateAsync(user, registerUserDto.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => new Error(ErrorCodes.BadRequest, e.Description)).ToArray();
            return Result<RegisteredUserDto>.BadRequest(errors);
        }
        var registeredUser = new RegisteredUserDto
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Id = user.Id
        };
        return Result<RegisteredUserDto>.Success(registeredUser);
    }

    // Login
    public async Task<Result<string>> LoginAsync(LoginDto loginDto)
    {
        var user = await userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            return Result<string>.Failure(new Error(ErrorCodes.BadRequest, "Invalid credentials."));
        }
        var valid = await userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!valid)
        {
            return Result<string>.Failure(new Error(ErrorCodes.BadRequest, "Invalid credentials."));
        }

        var token = await GenerateToken(user);

        return Result<string>.Success(token);
    }

    // Generate Token Function
    private async Task<string> GenerateToken(ApplicationUser user)
    {
        // set user claims
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, user.Id),
            new (JwtRegisteredClaimNames.Email, user.Email),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new (JwtRegisteredClaimNames.Name, user.FullName)
        };
        // set user role claims
        var roles = await userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();

        claims = claims.Union(roleClaims).ToList();

        // set jwt key credentials
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"] ?? string.Empty));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // create encoded token
        var token = new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                claims : claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(configuration["JwtSettings:DurationInMinutes"])),
                signingCredentials: credentials
            );

        // return token

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
