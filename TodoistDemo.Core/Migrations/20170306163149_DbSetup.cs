using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TodoistDemo.Core.Migrations
{
    public partial class DbSetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AssignedBy = table.Column<int>(nullable: true),
                    Checked = table.Column<int>(nullable: false),
                    Collapsed = table.Column<int>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    DateAdded = table.Column<string>(nullable: true),
                    DateLanguage = table.Column<string>(nullable: true),
                    DayOrder = table.Column<int>(nullable: false),
                    FormattedDate = table.Column<string>(nullable: true),
                    InHistory = table.Column<int>(nullable: false),
                    Indent = table.Column<int>(nullable: false),
                    IsArchived = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    ItemOrder = table.Column<int>(nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Avatar = table.Column<string>(nullable: true),
                    AvatarBig = table.Column<string>(nullable: true),
                    AvatarMedium = table.Column<string>(nullable: true),
                    AvatarSmall = table.Column<string>(nullable: true),
                    CompletedCount = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(nullable: true),
                    SortOrder = table.Column<int>(nullable: false),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
