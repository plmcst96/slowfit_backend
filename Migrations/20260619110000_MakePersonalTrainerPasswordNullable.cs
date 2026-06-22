using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using slowfit.DBModels;

#nullable disable

namespace slowfit.Migrations
{
    [DbContext(typeof(SlowFitContext))]
    [Migration("20260619110000_MakePersonalTrainerPasswordNullable")]
    public partial class MakePersonalTrainerPasswordNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Il PT viene creato senza password (verrà impostata dal flusso email/PATCH).
            migrationBuilder.Sql("""
                IF COL_LENGTH(N'[personalTrainer]', N'password') IS NOT NULL
                    ALTER TABLE [personalTrainer] ALTER COLUMN [password] VARCHAR(MAX) NULL;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH(N'[personalTrainer]', N'password') IS NOT NULL
                BEGIN
                    UPDATE [personalTrainer] SET [password] = '' WHERE [password] IS NULL;
                    ALTER TABLE [personalTrainer] ALTER COLUMN [password] VARCHAR(MAX) NOT NULL;
                END
                """);
        }
    }
}
