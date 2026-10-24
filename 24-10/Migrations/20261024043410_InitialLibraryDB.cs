using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLTV_Project.Migrations
{
    /// <inheritdoc />
    public partial class InitialLibraryDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TacGias",
                columns: table => new
                {
                    TacGiaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenTacGia = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TacGias", x => x.TacGiaId);
                });

            migrationBuilder.CreateTable(
                name: "Sachs",
                columns: table => new
                {
                    SachId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenSach = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TacGiaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sachs", x => x.SachId);
                    table.ForeignKey(
                        name: "FK_Sachs_TacGias_TacGiaId",
                        column: x => x.TacGiaId,
                        principalTable: "TacGias",
                        principalColumn: "TacGiaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sachs_TacGiaId",
                table: "Sachs",
                column: "TacGiaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sachs");

            migrationBuilder.DropTable(
                name: "TacGias");
        }
    }
}
