using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using slowfit.DBModels;

#nullable disable

namespace slowfit.Migrations
{
    [DbContext(typeof(SlowFitContext))]
    [Migration("20260622120000_AddPersonalTrainerPasswordSetupToken")]
    public partial class AddPersonalTrainerPasswordSetupToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "passwordSetupTokenHash",
                table: "personalTrainer",
                type: "varchar(256)",
                unicode: false,
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "passwordSetupTokenExpiresAt",
                table: "personalTrainer",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "passwordSetupTokenHash",
                table: "personalTrainer");

            migrationBuilder.DropColumn(
                name: "passwordSetupTokenExpiresAt",
                table: "personalTrainer");
        }
    }
}
