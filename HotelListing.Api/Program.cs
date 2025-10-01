//using HotelListing.Api.Contracts;
//using HotelListing.Api.MappingProfiles;
//using HotelListing.Api.Services;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;
//using Microsoft.OpenApi.Models;
//using HotelListing.Api.Common.Models;
//using HotelListing.App.Domain;

//var builder = WebApplication.CreateBuilder(args);
//var jwtKey = builder.Configuration["Jwt:Key"];
//var jwtIssuer = builder.Configuration["Jwt:Issuer"];
//var jwtAudience = builder.Configuration["Jwt:Audience"];
//var connectionString = builder.Configuration.GetConnectionString("HotelListingDbConnectionString");

//builder.Services.AddDbContext<HotelListingDbContext>(options =>
//    options.UseNpgsql(connectionString));
//builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
//    .AddRoles<IdentityRole>()
//    .AddEntityFrameworkStores<HotelListingDbContext>();
//builder.Services.AddHttpContextAccessor();
//var jwtSettings = builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
//if (string.IsNullOrWhiteSpace(jwtKey))
//{
//    throw new InvalidOperationException("JwtSettings: Key is not configured.");
//}
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
//        ValidAudience = builder.Configuration["JwtSettings:Audience"],
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"] ?? string.Empty)),
//        ClockSkew = TimeSpan.Zero
//    };
//});
//builder.Services.AddAuthorization();
//builder.Services.AddScoped<ICountriesService, CountriesService>();
//builder.Services.AddScoped<IHotelsServices, HotelsServices>();
//builder.Services.AddScoped<IUsersService, UsersService>();
//builder.Services.AddScoped<IBookingService, BookingService>();
//builder.Services.AddAutoMapper(cfg =>
//{
//    cfg.AddProfile<HotelMappingProfile>();
//    cfg.AddProfile<CountryMappingProfile>();
//});
//builder.Services.AddControllers().AddJsonOptions(opt =>
//{
//    opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
//});
//builder.Services.AddOpenApi();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(options =>
//{
//    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

//    var scheme = new OpenApiSecurityScheme
//    {
//        Name = "Authorization",
//        Type = SecuritySchemeType.Http,
//        Scheme = "bearer",
//        BearerFormat = "JWT",
//        In = ParameterLocation.Header,
//        Description = "Enter: Bearer {your token}"
//    };

//    options.AddSecurityDefinition("Bearer", scheme);
//    options.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }}, Array.Empty<string>() }
//    });
//});

//var app = builder.Build();
//app.MapGroup("api/defaultauth").MapIdentityApi<ApplicationUser>();

//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
//app.UseHttpsRedirection();
//app.UseAuthentication();
//app.UseAuthorization();
//app.MapControllers();
//app.Run();





























using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using HotelListing.App.Domain;
using HotelListing.App.Application.MappingProfiles;
using HotelListing.App.Application.Contracts;
using HotelListing.App.Application.Services;
using HotelListing.Api.Common.Models.Config;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Database connection
var connectionString = builder.Configuration.GetConnectionString("HotelListingDbConnectionString");
builder.Services.AddDbContext<HotelListingDbContext>(options =>
    options.UseNpgsql(connectionString));

// 🔹 Identity
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<HotelListingDbContext>();

builder.Services.AddHttpContextAccessor();

// 🔹 Bind JwtSettings from configuration
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

if (jwtSettings == null || string.IsNullOrWhiteSpace(jwtSettings.Key))
{
    throw new InvalidOperationException("JwtSettings: Key is not configured.");
}

// 🔹 Authentication & JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// 🔹 Application Services
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IHotelsServices, HotelsServices>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IBookingService, BookingService>();

// 🔹 AutoMapper Profiles
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<HotelMappingProfile>();
    cfg.AddProfile<CountryMappingProfile>();
});

// 🔹 Controllers & JSON Options
builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// 🔹 Swagger / OpenAPI
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    var scheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your token}"
    };

    options.AddSecurityDefinition("Bearer", scheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// 🔹 Identity API endpoints
app.MapGroup("api/defaultauth").MapIdentityApi<ApplicationUser>();

// 🔹 Development tools
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
