using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using slowfit.DBModels;

#nullable disable

namespace slowfit.Migrations
{
    [DbContext(typeof(SlowFitContext))]
    [Migration("20260615091000_CleanDateTypesAndSeedRoles")]
    public partial class CleanDateTypesAndSeedRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE [appointment] SET [date] = CONVERT(varchar(19), GETDATE(), 120) WHERE TRY_CONVERT(datetime2, [date], 120) IS NULL;
                ALTER TABLE [appointment] ALTER COLUMN [date] datetime2 NOT NULL;

                UPDATE [billing] SET [date] = CONVERT(varchar(10), GETDATE(), 23) WHERE TRY_CONVERT(date, [date], 23) IS NULL;
                ALTER TABLE [billing] ALTER COLUMN [date] date NOT NULL;

                UPDATE [measure] SET [collectPeriod] = CONVERT(varchar(10), GETDATE(), 23) WHERE TRY_CONVERT(date, [collectPeriod], 23) IS NULL;
                DECLARE @measureDefault sysname;
                SELECT @measureDefault = [d].[name]
                FROM [sys].[default_constraints] [d]
                INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
                WHERE [d].[parent_object_id] = OBJECT_ID(N'[measure]') AND [c].[name] = N'collectPeriod';
                IF @measureDefault IS NOT NULL EXEC(N'ALTER TABLE [measure] DROP CONSTRAINT [' + @measureDefault + ']');
                ALTER TABLE [measure] ALTER COLUMN [collectPeriod] date NOT NULL;

                UPDATE [nutrition] SET [creationDate] = NULL WHERE [creationDate] IS NOT NULL AND TRY_CONVERT(date, [creationDate], 23) IS NULL;
                UPDATE [nutrition] SET [expirationDate] = NULL WHERE [expirationDate] IS NOT NULL AND TRY_CONVERT(date, [expirationDate], 23) IS NULL;
                DECLARE @nutritionCreationDefault sysname;
                SELECT @nutritionCreationDefault = [d].[name]
                FROM [sys].[default_constraints] [d]
                INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
                WHERE [d].[parent_object_id] = OBJECT_ID(N'[nutrition]') AND [c].[name] = N'creationDate';
                IF @nutritionCreationDefault IS NOT NULL EXEC(N'ALTER TABLE [nutrition] DROP CONSTRAINT [' + @nutritionCreationDefault + ']');
                ALTER TABLE [nutrition] ALTER COLUMN [creationDate] date NULL;
                ALTER TABLE [nutrition] ALTER COLUMN [expirationDate] date NULL;

                UPDATE [product] SET [expirationDate] = CONVERT(varchar(10), GETDATE(), 23) WHERE TRY_CONVERT(date, [expirationDate], 23) IS NULL;
                ALTER TABLE [product] ALTER COLUMN [expirationDate] date NOT NULL;

                UPDATE [training] SET [creationDate] = NULL WHERE [creationDate] IS NOT NULL AND TRY_CONVERT(date, [creationDate], 23) IS NULL;
                UPDATE [training] SET [endDate] = NULL WHERE [endDate] IS NOT NULL AND TRY_CONVERT(date, [endDate], 23) IS NULL;
                DECLARE @trainingCreationDefault sysname;
                SELECT @trainingCreationDefault = [d].[name]
                FROM [sys].[default_constraints] [d]
                INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
                WHERE [d].[parent_object_id] = OBJECT_ID(N'[training]') AND [c].[name] = N'creationDate';
                IF @trainingCreationDefault IS NOT NULL EXEC(N'ALTER TABLE [training] DROP CONSTRAINT [' + @trainingCreationDefault + ']');
                ALTER TABLE [training] ALTER COLUMN [creationDate] date NULL;
                ALTER TABLE [training] ALTER COLUMN [endDate] date NULL;

                UPDATE [user] SET [birthDate] = NULL WHERE [birthDate] IS NOT NULL AND TRY_CONVERT(date, [birthDate], 23) IS NULL;
                ALTER TABLE [user] ALTER COLUMN [birthDate] date NULL;

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
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE [appointment] ALTER COLUMN [date] varchar(50) NOT NULL;
                ALTER TABLE [billing] ALTER COLUMN [date] varchar(50) NOT NULL;
                ALTER TABLE [measure] ALTER COLUMN [collectPeriod] varchar(50) NOT NULL;
                ALTER TABLE [nutrition] ALTER COLUMN [creationDate] varchar(50) NULL;
                ALTER TABLE [nutrition] ALTER COLUMN [expirationDate] varchar(50) NULL;
                ALTER TABLE [product] ALTER COLUMN [expirationDate] varchar(50) NOT NULL;
                ALTER TABLE [training] ALTER COLUMN [creationDate] varchar(50) NULL;
                ALTER TABLE [training] ALTER COLUMN [endDate] varchar(50) NULL;
                ALTER TABLE [user] ALTER COLUMN [birthDate] varchar(50) NULL;
                """);
        }
    }
}
