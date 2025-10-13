using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace slowfit.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserAndRoleUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointment_user",
                table: "appointment");

            migrationBuilder.DropForeignKey(
                name: "FK_billing_user",
                table: "billing");

            migrationBuilder.DropForeignKey(
                name: "FK__mealIngre__ingre__220B0B18",
                table: "mealIngredient");

            migrationBuilder.DropForeignKey(
                name: "FK__mealIngre__mealI__2116E6DF",
                table: "mealIngredient");

            migrationBuilder.DropForeignKey(
                name: "FK_measure_user",
                table: "measure");

            migrationBuilder.DropForeignKey(
                name: "FK_order_user",
                table: "order");

            migrationBuilder.DropForeignKey(
                name: "FK_responseQuiz_user",
                table: "responseQuiz");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "PK__mealIngr__6F15A6E8793153BC",
                table: "mealIngredient");

            migrationBuilder.DropColumn(
                name: "ingredientId",
                table: "meal");

            migrationBuilder.AlterColumn<int>(
                name: "userId",
                table: "user",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "user",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "user",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "user",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "user",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "user",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "user",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "user",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "user",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "user",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "user",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "user",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "user",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "user",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "user",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "roleUser",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "roleUser",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "roleUser",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "roleUser",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "unit",
                table: "mealIngredient",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "quantity",
                table: "mealIngredient",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<int>(
                name: "mealIngredientId",
                table: "mealIngredient",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user",
                table: "user",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Table_1",
                table: "mealIngredient",
                column: "mealIngredientId");

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_roleUser_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roleUser",
                        principalColumn: "roleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_roleUser_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roleUser",
                        principalColumn: "roleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "user",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "user",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "roleUser",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_mealIngredient_mealId",
                table: "mealIngredient",
                column: "mealId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_appointment_user",
                table: "appointment",
                column: "userId",
                principalTable: "user",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_billing_user",
                table: "billing",
                column: "userId",
                principalTable: "user",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_mealIngredient_ingredient",
                table: "mealIngredient",
                column: "ingredientId",
                principalTable: "ingredient",
                principalColumn: "ingredientId");

            migrationBuilder.AddForeignKey(
                name: "FK_mealIngredient_meal",
                table: "mealIngredient",
                column: "mealId",
                principalTable: "meal",
                principalColumn: "mealId");

            migrationBuilder.AddForeignKey(
                name: "FK_measure_user",
                table: "measure",
                column: "userId",
                principalTable: "user",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_order_user",
                table: "order",
                column: "userId",
                principalTable: "user",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_responseQuiz_user",
                table: "responseQuiz",
                column: "userId",
                principalTable: "user",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointment_user",
                table: "appointment");

            migrationBuilder.DropForeignKey(
                name: "FK_billing_user",
                table: "billing");

            migrationBuilder.DropForeignKey(
                name: "FK_mealIngredient_ingredient",
                table: "mealIngredient");

            migrationBuilder.DropForeignKey(
                name: "FK_mealIngredient_meal",
                table: "mealIngredient");

            migrationBuilder.DropForeignKey(
                name: "FK_measure_user",
                table: "measure");

            migrationBuilder.DropForeignKey(
                name: "FK_order_user",
                table: "order");

            migrationBuilder.DropForeignKey(
                name: "FK_responseQuiz_user",
                table: "responseQuiz");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user",
                table: "user");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "user");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "user");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "roleUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Table_1",
                table: "mealIngredient");

            migrationBuilder.DropIndex(
                name: "IX_mealIngredient_mealId",
                table: "mealIngredient");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "user");

            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "user");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "user");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "user");

            migrationBuilder.DropColumn(
                name: "LockoutEnabled",
                table: "user");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "user");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "user");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "user");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "user");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "user");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "user");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "user");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "user");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "user");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "roleUser");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "roleUser");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "roleUser");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "roleUser");

            migrationBuilder.DropColumn(
                name: "mealIngredientId",
                table: "mealIngredient");

            migrationBuilder.AlterColumn<int>(
                name: "userId",
                table: "user",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "unit",
                table: "mealIngredient",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<double>(
                name: "quantity",
                table: "mealIngredient",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ingredientId",
                table: "meal",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_user",
                table: "user",
                column: "userId");

            migrationBuilder.AddPrimaryKey(
                name: "PK__mealIngr__6F15A6E8793153BC",
                table: "mealIngredient",
                columns: new[] { "mealId", "ingredientId" });

            migrationBuilder.AddForeignKey(
                name: "FK_appointment_user",
                table: "appointment",
                column: "userId",
                principalTable: "user",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_billing_user",
                table: "billing",
                column: "userId",
                principalTable: "user",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK__mealIngre__ingre__220B0B18",
                table: "mealIngredient",
                column: "ingredientId",
                principalTable: "ingredient",
                principalColumn: "ingredientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__mealIngre__mealI__2116E6DF",
                table: "mealIngredient",
                column: "mealId",
                principalTable: "meal",
                principalColumn: "mealId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_measure_user",
                table: "measure",
                column: "userId",
                principalTable: "user",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_order_user",
                table: "order",
                column: "userId",
                principalTable: "user",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_responseQuiz_user",
                table: "responseQuiz",
                column: "userId",
                principalTable: "user",
                principalColumn: "userId");
        }
    }
}
