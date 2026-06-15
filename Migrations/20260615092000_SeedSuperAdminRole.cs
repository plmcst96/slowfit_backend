using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace slowfit.Migrations
{
    public partial class SeedSuperAdminRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
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
            migrationBuilder.Sql("DELETE FROM [roleUser] WHERE [roleId] = 3 AND [roleName] = 'Super Admin';");
        }
    }
}
