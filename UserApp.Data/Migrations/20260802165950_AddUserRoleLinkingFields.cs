using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRoleLinkingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KullaniciId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YonetilenDepartmanId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KullaniciId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "YonetilenDepartmanId",
                table: "AspNetUsers");
        }
    }
}
