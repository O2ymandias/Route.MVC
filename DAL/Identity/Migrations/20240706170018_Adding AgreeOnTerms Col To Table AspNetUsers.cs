using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Identity.Migrations
{
    /// <inheritdoc />
    public partial class AddingAgreeOnTermsColToTableAspNetUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AgreeOnTerms",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgreeOnTerms",
                table: "AspNetUsers");
        }
    }
}
