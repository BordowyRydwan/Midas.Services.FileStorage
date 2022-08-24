using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Messages", x => x.Id); });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "Content" },
                values: new object[,]
                {
                    { 1m, "Test message 1" },
                    { 2m, "Test message 2" },
                    { 3m, "Test message 3" },
                    { 4m, "Test message 4" },
                    { 5m, "Test message 5" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");
        }
    }
}