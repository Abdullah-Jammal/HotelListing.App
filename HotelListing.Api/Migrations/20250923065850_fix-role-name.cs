using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelListing.Api.Migrations
{
    /// <inheritdoc />
    public partial class fixrolename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b5603e82-ab80-4003-8979-dc8bb358a62a",
                column: "NormalizedName",
                value: "ADMINISTRATOR");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b5603e82-ab80-4003-8979-dc8bb358a62a",
                column: "NormalizedName",
                value: "ADMINSTRATOR");
        }
    }
}
