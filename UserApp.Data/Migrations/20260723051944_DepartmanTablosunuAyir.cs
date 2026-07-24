using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class DepartmanTablosunuAyir : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Departman",
                table: "Kullanicilar");

            migrationBuilder.AddColumn<int>(
                name: "DepartmanId",
                table: "Kullanicilar",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "Departmanlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departmanlar", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Departmanlar",
                columns: new[] { "Id", "Ad" },
                values: new object[,]
                {
                    { 1, "IT" },
                    { 2, "Muhasebe" },
                    { 3, "İnsan Kaynakları" },
                    { 4, "Satış" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_DepartmanId",
                table: "Kullanicilar",
                column: "DepartmanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Kullanicilar_Departmanlar_DepartmanId",
                table: "Kullanicilar",
                column: "DepartmanId",
                principalTable: "Departmanlar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kullanicilar_Departmanlar_DepartmanId",
                table: "Kullanicilar");

            migrationBuilder.DropTable(
                name: "Departmanlar");

            migrationBuilder.DropIndex(
                name: "IX_Kullanicilar_DepartmanId",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "DepartmanId",
                table: "Kullanicilar");

            migrationBuilder.AddColumn<string>(
                name: "Departman",
                table: "Kullanicilar",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
