using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the Ioc container.

builder.Services.AddDbContext<HotelListingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("HotelListingDbConnectionString")));

builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IHotelsServices, HotelsServices>();

builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
