using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CalendarAPI.Migrations
{
    /// <inheritdoc />
    public partial class GameRecordTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d393f3dd-38df-4cdd-a7ef-42dfae9731b3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dc271b47-8760-45d3-8d53-4c7bee331151");

            migrationBuilder.CreateTable(
                name: "GameRecords",
                columns: table => new
                {
                    GameId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WonGame = table.Column<bool>(type: "bit", nullable: false),
                    PlayedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameRecords", x => x.GameId);
                    table.ForeignKey(
                        name: "FK_GameRecords_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5e057f41-7f04-4e96-8427-ed5bdaac80d4", null, "Admin", "ADMIN" },
                    { "adeaf8ba-de69-45bc-aa9c-ef2e825eba8d", null, "User", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameRecords_UserId",
                table: "GameRecords",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameRecords");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5e057f41-7f04-4e96-8427-ed5bdaac80d4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "adeaf8ba-de69-45bc-aa9c-ef2e825eba8d");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "d393f3dd-38df-4cdd-a7ef-42dfae9731b3", null, "Admin", "ADMIN" },
                    { "dc271b47-8760-45d3-8d53-4c7bee331151", null, "User", "USER" }
                });
        }
    }
}
