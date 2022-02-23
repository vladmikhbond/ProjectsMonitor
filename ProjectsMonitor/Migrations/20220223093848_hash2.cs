using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectsMonitor.Migrations
{
    public partial class hash2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Hash",
                table: "Hash");

            migrationBuilder.RenameTable(
                name: "Hash",
                newName: "Hashs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Hashs",
                table: "Hashs",
                columns: new[] { "Tag", "UserName" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Hashs",
                table: "Hashs");

            migrationBuilder.RenameTable(
                name: "Hashs",
                newName: "Hash");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Hash",
                table: "Hash",
                columns: new[] { "Tag", "UserName" });
        }
    }
}
