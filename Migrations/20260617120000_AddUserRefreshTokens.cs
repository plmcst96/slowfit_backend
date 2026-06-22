using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using slowfit.DBModels;

#nullable disable

namespace slowfit.Migrations
{
    [DbContext(typeof(SlowFitContext))]
    [Migration("20260617120000_AddUserRefreshTokens")]
    public partial class AddUserRefreshTokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "refreshTokenExpiresAt",
                table: "user",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "refreshTokenHash",
                table: "user",
                type: "varchar(256)",
                unicode: false,
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "refreshTokenRevokedAt",
                table: "user",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "refreshTokenExpiresAt",
                table: "user");

            migrationBuilder.DropColumn(
                name: "refreshTokenHash",
                table: "user");

            migrationBuilder.DropColumn(
                name: "refreshTokenRevokedAt",
                table: "user");
        }
    }
}
