using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Autoszerelo_API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Munkak",
                columns: table => new
                {
                    MunkaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UgyfelId = table.Column<int>(type: "int", nullable: false),
                    Rendszam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GyartasiEv = table.Column<int>(type: "int", nullable: false),
                    Kategoria = table.Column<int>(type: "int", nullable: false),
                    HibaLeiras = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    HibaSulyossaga = table.Column<int>(type: "int", nullable: false),
                    Allapot = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Munkak", x => x.MunkaId);
                });

            migrationBuilder.CreateTable(
                name: "Ugyfelek",
                columns: table => new
                {
                    UgyfelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nev = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Lakcim = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ugyfelek", x => x.UgyfelId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Munkak");

            migrationBuilder.DropTable(
                name: "Ugyfelek");
        }
    }
}
