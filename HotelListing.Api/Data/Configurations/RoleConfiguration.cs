using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Api.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole
            {
                Id = "b5603e82-ab80-4003-8979-dc8bb358a62a",
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            },
            new
            {
                Id = "951a85f7-33ef-4093-ada0-4f1184104f88",
                Name = "User",
                NormalizedName = "USER"
            }
        );
    }
}
