using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectsMonitor.Migrations
{
    public partial class hash4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Hashs",
                table: "Hashs");

            migrationBuilder.DropColumn(
                name: "Tag",
                table: "Hashs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Hashs",
                table: "Hashs",
                column: "UserName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Hashs",
                table: "Hashs");

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "Hashs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Hashs",
                table: "Hashs",
                columns: new[] { "Tag", "UserName" });
        }
    }
}
