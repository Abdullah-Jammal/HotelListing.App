using HotelListing.Api.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HotelListing.Api.AuthrizationFilter;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class HotelOrSystemAdminAttribute : TypeFilterAttribute
{
    public HotelOrSystemAdminAttribute() : base(typeof(HotelOrSystemAdminFilter))
    {
    }
}

public class HotelOrSystemAdminFilter(HotelListingDbContext dbContext) : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var httpUser = context.HttpContext.User;
        if (httpUser?.Identity?.IsAuthenticated == false)
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        if(httpUser!.IsInRole("Administrator"))
        {
            return;
        }

        var userId = httpUser.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
        httpUser?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            context.Result = new ForbidResult();
            return;
        }
        context.RouteData.Values.TryGetValue("hotelId", out var hotelIdObj);
        int.TryParse(hotelIdObj?.ToString(), out int hotelId);
        if (hotelId == 0)
        {
            context.Result = new ForbidResult();
            return;
        }
        var isHotelAdminUser = await dbContext.Admins.AnyAsync(a => a.HotelId == hotelId && a.UserId == userId);
        if (!isHotelAdminUser) { 
            context.Result = new ForbidResult();
            return;
        }
    }
}