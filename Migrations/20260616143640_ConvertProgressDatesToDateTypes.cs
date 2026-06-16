using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using slowfit.DBModels;

#nullable disable

namespace slowfit.Migrations
{
    [DbContext(typeof(SlowFitContext))]
    [Migration("20260616143640_ConvertProgressDatesToDateTypes")]
    public partial class ConvertProgressDatesToDateTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[progressTraining]', N'U') IS NOT NULL
                BEGIN
                    UPDATE [progressTraining]
                    SET [createdAt] = CONVERT(varchar(33), SYSUTCDATETIME(), 126)
                    WHERE [createdAt] IS NULL OR COALESCE(TRY_CONVERT(datetime2, [createdAt], 126), TRY_CONVERT(datetime2, [createdAt], 127), TRY_CONVERT(datetime2, [createdAt])) IS NULL;

                    UPDATE [progressTraining]
                    SET [dateOfProgress] = CONVERT(varchar(10), CAST(SYSUTCDATETIME() AS date), 23)
                    WHERE [dateOfProgress] IS NULL OR COALESCE(TRY_CONVERT(date, [dateOfProgress], 23), TRY_CONVERT(date, [dateOfProgress], 126), TRY_CONVERT(date, [dateOfProgress], 127), TRY_CONVERT(date, [dateOfProgress])) IS NULL;

                    ALTER TABLE [progressTraining] ALTER COLUMN [createdAt] datetime2 NOT NULL;
                    ALTER TABLE [progressTraining] ALTER COLUMN [dateOfProgress] date NOT NULL;
                END

                IF OBJECT_ID(N'[progressNutrition]', N'U') IS NOT NULL
                BEGIN
                    UPDATE [progressNutrition]
                    SET [createdAt] = NULL
                    WHERE [createdAt] IS NOT NULL AND COALESCE(TRY_CONVERT(datetime2, [createdAt], 126), TRY_CONVERT(datetime2, [createdAt], 127), TRY_CONVERT(datetime2, [createdAt])) IS NULL;

                    UPDATE [progressNutrition]
                    SET [dateOfProgress] = CONVERT(varchar(10), CAST(SYSUTCDATETIME() AS date), 23)
                    WHERE [dateOfProgress] IS NULL OR COALESCE(TRY_CONVERT(date, [dateOfProgress], 23), TRY_CONVERT(date, [dateOfProgress], 126), TRY_CONVERT(date, [dateOfProgress], 127), TRY_CONVERT(date, [dateOfProgress])) IS NULL;

                    ALTER TABLE [progressNutrition] ALTER COLUMN [createdAt] datetime2 NULL;
                    ALTER TABLE [progressNutrition] ALTER COLUMN [dateOfProgress] date NOT NULL;
                END
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[progressTraining]', N'U') IS NOT NULL
                BEGIN
                    ALTER TABLE [progressTraining] ALTER COLUMN [createdAt] varchar(50) NOT NULL;
                    ALTER TABLE [progressTraining] ALTER COLUMN [dateOfProgress] varchar(50) NOT NULL;
                END

                IF OBJECT_ID(N'[progressNutrition]', N'U') IS NOT NULL
                BEGIN
                    ALTER TABLE [progressNutrition] ALTER COLUMN [createdAt] varchar(50) NULL;
                    ALTER TABLE [progressNutrition] ALTER COLUMN [dateOfProgress] varchar(50) NOT NULL;
                END
                """);
        }
    }
}
