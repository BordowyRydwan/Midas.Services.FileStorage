using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class ChangeDbSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "file");

            migrationBuilder.RenameTable(
                name: "FileTypes",
                newName: "FileTypes",
                newSchema: "file");

            migrationBuilder.RenameTable(
                name: "Files",
                newName: "Files",
                newSchema: "file");

            migrationBuilder.RenameTable(
                name: "FileMetadatas",
                newName: "FileMetadatas",
                newSchema: "file");

            migrationBuilder.RenameTable(
                name: "FileDownloads",
                newName: "FileDownloads",
                newSchema: "file");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "FileTypes",
                schema: "file",
                newName: "FileTypes");

            migrationBuilder.RenameTable(
                name: "Files",
                schema: "file",
                newName: "Files");

            migrationBuilder.RenameTable(
                name: "FileMetadatas",
                schema: "file",
                newName: "FileMetadatas");

            migrationBuilder.RenameTable(
                name: "FileDownloads",
                schema: "file",
                newName: "FileDownloads");
        }
    }
}
