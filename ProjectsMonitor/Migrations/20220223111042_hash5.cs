using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectsMonitor.Migrations
{
    public partial class hash5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Hashs",
                table: "Hashs");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Hashs");

            migrationBuilder.AddColumn<int>(
                name: "GrId",
                table: "Hashs",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Hashs",
                table: "Hashs",
                column: "GrId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Hashs",
                table: "Hashs");

            migrationBuilder.DropColumn(
                name: "GrId",
                table: "Hashs");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Hashs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Hashs",
                table: "Hashs",
                column: "UserName");
        }
    }
}
