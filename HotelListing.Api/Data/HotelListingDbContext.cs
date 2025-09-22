using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Data;

public class HotelListingDbContext : IdentityDbContext<ApplicationUser>
{
    public HotelListingDbContext(DbContextOptions<HotelListingDbContext> option) : base(option)
    {
        
    }

    public DbSet<Country> Countries { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
}
