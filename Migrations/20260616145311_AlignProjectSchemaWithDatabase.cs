using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using slowfit.DBModels;

#nullable disable

namespace slowfit.Migrations
{
    [DbContext(typeof(SlowFitContext))]
    [Migration("20260616145311_AlignProjectSchemaWithDatabase")]
    public partial class AlignProjectSchemaWithDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                SET XACT_ABORT ON;

                IF OBJECT_ID(N'[roleUser]', N'U') IS NOT NULL
                BEGIN
                    UPDATE [roleUser] SET [roleName] = 'User' WHERE [roleId] = 1;
                    UPDATE [roleUser] SET [roleName] = 'Personal Trainer' WHERE [roleId] = 2;
                    UPDATE [roleUser] SET [roleName] = 'Super Admin' WHERE [roleId] = 3;

                    IF NOT EXISTS (SELECT 1 FROM [roleUser] WHERE [roleId] = 1)
                    BEGIN
                        SET IDENTITY_INSERT [roleUser] ON;
                        INSERT INTO [roleUser] ([roleId], [roleName]) VALUES (1, 'User');
                        SET IDENTITY_INSERT [roleUser] OFF;
                    END

                    IF NOT EXISTS (SELECT 1 FROM [roleUser] WHERE [roleId] = 2)
                    BEGIN
                        SET IDENTITY_INSERT [roleUser] ON;
                        INSERT INTO [roleUser] ([roleId], [roleName]) VALUES (2, 'Personal Trainer');
                        SET IDENTITY_INSERT [roleUser] OFF;
                    END

                    IF NOT EXISTS (SELECT 1 FROM [roleUser] WHERE [roleId] = 3)
                    BEGIN
                        SET IDENTITY_INSERT [roleUser] ON;
                        INSERT INTO [roleUser] ([roleId], [roleName]) VALUES (3, 'Super Admin');
                        SET IDENTITY_INSERT [roleUser] OFF;
                    END
                END

                IF OBJECT_ID(N'[user]', N'U') IS NOT NULL
                BEGIN
                    ALTER TABLE [user] ALTER COLUMN [password] varchar(max) NOT NULL;

                    UPDATE [user]
                    SET [birthDate] = NULL
                    WHERE [birthDate] IS NOT NULL AND COALESCE(TRY_CONVERT(date, [birthDate], 23), TRY_CONVERT(date, [birthDate], 126), TRY_CONVERT(date, [birthDate], 127), TRY_CONVERT(date, [birthDate])) IS NULL;

                    ALTER TABLE [user] ALTER COLUMN [birthDate] date NULL;
                END

                IF OBJECT_ID(N'[appointment]', N'U') IS NOT NULL
                BEGIN
                    UPDATE [appointment]
                    SET [date] = CONVERT(varchar(33), SYSUTCDATETIME(), 126)
                    WHERE [date] IS NULL OR COALESCE(TRY_CONVERT(datetime2, [date], 120), TRY_CONVERT(datetime2, [date], 126), TRY_CONVERT(datetime2, [date], 127), TRY_CONVERT(datetime2, [date])) IS NULL;

                    ALTER TABLE [appointment] ALTER COLUMN [date] datetime2 NOT NULL;
                END

                IF OBJECT_ID(N'[billing]', N'U') IS NOT NULL
                BEGIN
                    UPDATE [billing]
                    SET [date] = CONVERT(varchar(10), CAST(SYSUTCDATETIME() AS date), 23)
                    WHERE [date] IS NULL OR COALESCE(TRY_CONVERT(date, [date], 23), TRY_CONVERT(date, [date], 126), TRY_CONVERT(date, [date], 127), TRY_CONVERT(date, [date])) IS NULL;

                    ALTER TABLE [billing] ALTER COLUMN [date] date NOT NULL;
                END

                IF OBJECT_ID(N'[measure]', N'U') IS NOT NULL
                BEGIN
                    UPDATE [measure]
                    SET [collectPeriod] = CONVERT(varchar(10), CAST(SYSUTCDATETIME() AS date), 23)
                    WHERE [collectPeriod] IS NULL OR COALESCE(TRY_CONVERT(date, [collectPeriod], 23), TRY_CONVERT(date, [collectPeriod], 126), TRY_CONVERT(date, [collectPeriod], 127), TRY_CONVERT(date, [collectPeriod])) IS NULL;

                    DECLARE @measureDefault sysname;
                    SELECT @measureDefault = [d].[name]
                    FROM [sys].[default_constraints] [d]
                    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
                    WHERE [d].[parent_object_id] = OBJECT_ID(N'[measure]') AND [c].[name] = N'collectPeriod';
                    IF @measureDefault IS NOT NULL EXEC(N'ALTER TABLE [measure] DROP CONSTRAINT [' + @measureDefault + ']');

                    ALTER TABLE [measure] ALTER COLUMN [collectPeriod] date NOT NULL;
                END

                IF OBJECT_ID(N'[nutrition]', N'U') IS NOT NULL
                BEGIN
                    UPDATE [nutrition]
                    SET [creationDate] = NULL
                    WHERE [creationDate] IS NOT NULL AND COALESCE(TRY_CONVERT(date, [creationDate], 23), TRY_CONVERT(date, [creationDate], 126), TRY_CONVERT(date, [creationDate], 127), TRY_CONVERT(date, [creationDate])) IS NULL;

                    UPDATE [nutrition]
                    SET [expirationDate] = NULL
                    WHERE [expirationDate] IS NOT NULL AND COALESCE(TRY_CONVERT(date, [expirationDate], 23), TRY_CONVERT(date, [expirationDate], 126), TRY_CONVERT(date, [expirationDate], 127), TRY_CONVERT(date, [expirationDate])) IS NULL;

                    DECLARE @nutritionCreationDefault sysname;
                    SELECT @nutritionCreationDefault = [d].[name]
                    FROM [sys].[default_constraints] [d]
                    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
                    WHERE [d].[parent_object_id] = OBJECT_ID(N'[nutrition]') AND [c].[name] = N'creationDate';
                    IF @nutritionCreationDefault IS NOT NULL EXEC(N'ALTER TABLE [nutrition] DROP CONSTRAINT [' + @nutritionCreationDefault + ']');

                    ALTER TABLE [nutrition] ALTER COLUMN [creationDate] date NULL;
                    ALTER TABLE [nutrition] ALTER COLUMN [expirationDate] date NULL;
                END

                IF OBJECT_ID(N'[product]', N'U') IS NOT NULL
                BEGIN
                    UPDATE [product]
                    SET [expirationDate] = CONVERT(varchar(10), CAST(SYSUTCDATETIME() AS date), 23)
                    WHERE [expirationDate] IS NULL OR COALESCE(TRY_CONVERT(date, [expirationDate], 23), TRY_CONVERT(date, [expirationDate], 126), TRY_CONVERT(date, [expirationDate], 127), TRY_CONVERT(date, [expirationDate])) IS NULL;

                    ALTER TABLE [product] ALTER COLUMN [expirationDate] date NOT NULL;
                END

                IF OBJECT_ID(N'[training]', N'U') IS NOT NULL
                BEGIN
                    UPDATE [training]
                    SET [creationDate] = NULL
                    WHERE [creationDate] IS NOT NULL AND COALESCE(TRY_CONVERT(date, [creationDate], 23), TRY_CONVERT(date, [creationDate], 126), TRY_CONVERT(date, [creationDate], 127), TRY_CONVERT(date, [creationDate])) IS NULL;

                    UPDATE [training]
                    SET [endDate] = NULL
                    WHERE [endDate] IS NOT NULL AND COALESCE(TRY_CONVERT(date, [endDate], 23), TRY_CONVERT(date, [endDate], 126), TRY_CONVERT(date, [endDate], 127), TRY_CONVERT(date, [endDate])) IS NULL;

                    DECLARE @trainingCreationDefault sysname;
                    SELECT @trainingCreationDefault = [d].[name]
                    FROM [sys].[default_constraints] [d]
                    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
                    WHERE [d].[parent_object_id] = OBJECT_ID(N'[training]') AND [c].[name] = N'creationDate';
                    IF @trainingCreationDefault IS NOT NULL EXEC(N'ALTER TABLE [training] DROP CONSTRAINT [' + @trainingCreationDefault + ']');

                    ALTER TABLE [training] ALTER COLUMN [creationDate] date NULL;
                    ALTER TABLE [training] ALTER COLUMN [endDate] date NULL;
                END

                IF OBJECT_ID(N'[nutritionMeal]', N'U') IS NOT NULL
                BEGIN
                    DELETE FROM [nutritionMeal] WHERE [dayId] IS NULL;

                    DECLARE @nutritionMealPk sysname;
                    SELECT @nutritionMealPk = [kc].[name]
                    FROM [sys].[key_constraints] [kc]
                    WHERE [kc].[parent_object_id] = OBJECT_ID(N'[nutritionMeal]') AND [kc].[type] = 'PK';
                    IF @nutritionMealPk IS NOT NULL EXEC(N'ALTER TABLE [nutritionMeal] DROP CONSTRAINT [' + @nutritionMealPk + ']');

                    ALTER TABLE [nutritionMeal] ALTER COLUMN [dayId] int NOT NULL;
                    ALTER TABLE [nutritionMeal] ADD CONSTRAINT [PK_NutritionMeals] PRIMARY KEY ([nutritionId], [mealId], [dayId]);
                END

                IF OBJECT_ID(N'[detailExercise]', N'U') IS NOT NULL AND COL_LENGTH(N'[detailExercise]', N'kg') IS NULL
                BEGIN
                    ALTER TABLE [detailExercise] ADD [kg] decimal(5, 0) NULL;
                END
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[nutritionMeal]', N'U') IS NOT NULL
                BEGIN
                    DECLARE @nutritionMealPk sysname;
                    SELECT @nutritionMealPk = [kc].[name]
                    FROM [sys].[key_constraints] [kc]
                    WHERE [kc].[parent_object_id] = OBJECT_ID(N'[nutritionMeal]') AND [kc].[type] = 'PK';
                    IF @nutritionMealPk IS NOT NULL EXEC(N'ALTER TABLE [nutritionMeal] DROP CONSTRAINT [' + @nutritionMealPk + ']');

                    ALTER TABLE [nutritionMeal] ADD CONSTRAINT [PK_NutritionMeals] PRIMARY KEY ([nutritionId], [mealId]);
                END

                IF OBJECT_ID(N'[user]', N'U') IS NOT NULL
                BEGIN
                    ALTER TABLE [user] ALTER COLUMN [password] varchar(50) NOT NULL;
                    ALTER TABLE [user] ALTER COLUMN [birthDate] varchar(50) NULL;
                END

                IF OBJECT_ID(N'[appointment]', N'U') IS NOT NULL ALTER TABLE [appointment] ALTER COLUMN [date] varchar(50) NOT NULL;
                IF OBJECT_ID(N'[billing]', N'U') IS NOT NULL ALTER TABLE [billing] ALTER COLUMN [date] varchar(50) NOT NULL;
                IF OBJECT_ID(N'[measure]', N'U') IS NOT NULL ALTER TABLE [measure] ALTER COLUMN [collectPeriod] varchar(50) NOT NULL;
                IF OBJECT_ID(N'[nutrition]', N'U') IS NOT NULL ALTER TABLE [nutrition] ALTER COLUMN [creationDate] varchar(50) NULL;
                IF OBJECT_ID(N'[nutrition]', N'U') IS NOT NULL ALTER TABLE [nutrition] ALTER COLUMN [expirationDate] varchar(50) NULL;
                IF OBJECT_ID(N'[product]', N'U') IS NOT NULL ALTER TABLE [product] ALTER COLUMN [expirationDate] varchar(50) NOT NULL;
                IF OBJECT_ID(N'[training]', N'U') IS NOT NULL ALTER TABLE [training] ALTER COLUMN [creationDate] varchar(50) NULL;
                IF OBJECT_ID(N'[training]', N'U') IS NOT NULL ALTER TABLE [training] ALTER COLUMN [endDate] varchar(50) NULL;
                """);
        }
    }
}
