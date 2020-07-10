using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FirewallManager.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Server = table.Column<string>(maxLength: 50, nullable: false),
                    Ipaddr = table.Column<string>(maxLength: 500, nullable: false),
                    RuleName = table.Column<string>(maxLength: 500, nullable: false),
                    Processed = table.Column<bool>(nullable: false, defaultValue: false),
                    RuleId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rules_Rules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "Rules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rules_Created",
                table: "Rules",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_Rules_Processed",
                table: "Rules",
                column: "Processed");

            migrationBuilder.CreateIndex(
                name: "IX_Rules_RuleId",
                table: "Rules",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Rules_RuleName",
                table: "Rules",
                column: "RuleName");

            migrationBuilder.CreateIndex(
                name: "IX_Rules_Server_Ipaddr",
                table: "Rules",
                columns: new[] { "Server", "Ipaddr" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rules");
        }
    }
}
