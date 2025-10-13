using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace slowfit.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bodyPart",
                columns: table => new
                {
                    bodyPartId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    bodyPartName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bodyPart", x => x.bodyPartId);
                });

            migrationBuilder.CreateTable(
                name: "categoryOfDay",
                columns: table => new
                {
                    categoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    momentOfDay = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categoryOfDay", x => x.categoryId);
                });

            migrationBuilder.CreateTable(
                name: "ingredient",
                columns: table => new
                {
                    ingredientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    calories = table.Column<int>(type: "int", nullable: true),
                    protein = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    fats = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    carbohydrate = table.Column<decimal>(type: "decimal(5,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingredient", x => x.ingredientId);
                });

            migrationBuilder.CreateTable(
                name: "inputType",
                columns: table => new
                {
                    inputTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    inputTypeName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inputType", x => x.inputTypeId);
                });

            migrationBuilder.CreateTable(
                name: "levelTraining",
                columns: table => new
                {
                    levelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    levelString = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_levelTraining", x => x.levelId);
                });

            migrationBuilder.CreateTable(
                name: "locationTraining",
                columns: table => new
                {
                    locationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    locationString = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_locationTraining", x => x.locationId);
                });

            migrationBuilder.CreateTable(
                name: "paymentType",
                columns: table => new
                {
                    paymentTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    paymentTypeName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_paymentType", x => x.paymentTypeId);
                });

            migrationBuilder.CreateTable(
                name: "question",
                columns: table => new
                {
                    questionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    questionString = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_question", x => x.questionId);
                });

            migrationBuilder.CreateTable(
                name: "roleUser",
                columns: table => new
                {
                    roleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    roleName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roleUser", x => x.roleId);
                });

            migrationBuilder.CreateTable(
                name: "typeNutrition",
                columns: table => new
                {
                    typeNutritionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    typeNutritionName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_typeNutrition", x => x.typeNutritionId);
                });

            migrationBuilder.CreateTable(
                name: "typePlan",
                columns: table => new
                {
                    typePlaneId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    typePlaneName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_typePlan", x => x.typePlaneId);
                });

            migrationBuilder.CreateTable(
                name: "typeTrainig",
                columns: table => new
                {
                    typeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    typeName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_typeTrainig", x => x.typeId);
                });

            migrationBuilder.CreateTable(
                name: "meal",
                columns: table => new
                {
                    mealId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    recipe = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    ingredientId = table.Column<int>(type: "int", nullable: false),
                    calories = table.Column<int>(type: "int", nullable: false),
                    protein = table.Column<int>(type: "int", nullable: false),
                    fats = table.Column<int>(type: "int", nullable: false),
                    carbohydrate = table.Column<int>(type: "int", nullable: false),
                    preparingTime = table.Column<int>(type: "int", nullable: false),
                    imageMeal = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    difficulty = table.Column<int>(type: "int", nullable: true),
                    categoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_meal", x => x.mealId);
                    table.ForeignKey(
                        name: "FK_meal_categoryOfDay",
                        column: x => x.categoryId,
                        principalTable: "categoryOfDay",
                        principalColumn: "categoryId");
                });

            migrationBuilder.CreateTable(
                name: "exercise",
                columns: table => new
                {
                    exerciseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    urlVideo = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    image = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    typeTrainingId = table.Column<int>(type: "int", nullable: false),
                    locationTrainingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercise", x => x.exerciseId);
                    table.ForeignKey(
                        name: "FK_exercise_locationTraining",
                        column: x => x.locationTrainingId,
                        principalTable: "locationTraining",
                        principalColumn: "locationId");
                });

            migrationBuilder.CreateTable(
                name: "answer",
                columns: table => new
                {
                    answerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    answerString = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    questionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_answer", x => x.answerId);
                    table.ForeignKey(
                        name: "FK_answer_question",
                        column: x => x.questionId,
                        principalTable: "question",
                        principalColumn: "questionId");
                });

            migrationBuilder.CreateTable(
                name: "quiz",
                columns: table => new
                {
                    quizId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    questionId = table.Column<int>(type: "int", nullable: false),
                    inputTypeId = table.Column<int>(type: "int", nullable: true),
                    input = table.Column<bool>(type: "bit", nullable: true),
                    singleResponse = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quiz", x => x.quizId);
                    table.ForeignKey(
                        name: "FK_quiz_inputType",
                        column: x => x.inputTypeId,
                        principalTable: "inputType",
                        principalColumn: "inputTypeId");
                    table.ForeignKey(
                        name: "FK_quiz_question",
                        column: x => x.questionId,
                        principalTable: "question",
                        principalColumn: "questionId");
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    userId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    firstName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    surname = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    address = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    city = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    country = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    province = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    zipCode = table.Column<int>(type: "int", nullable: true),
                    roleId = table.Column<int>(type: "int", nullable: true, defaultValue: 1),
                    birthDate = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    nutritionId = table.Column<int>(type: "int", nullable: true),
                    trainingId = table.Column<int>(type: "int", nullable: true),
                    ptId = table.Column<int>(type: "int", nullable: true),
                    imageProfile = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    phone = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.userId);
                    table.ForeignKey(
                        name: "FK_user_roleUser",
                        column: x => x.roleId,
                        principalTable: "roleUser",
                        principalColumn: "roleId");
                });

            migrationBuilder.CreateTable(
                name: "nutrition",
                columns: table => new
                {
                    nutritionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    expirationDate = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    mealId = table.Column<int>(type: "int", nullable: false),
                    typeNutritionId = table.Column<int>(type: "int", nullable: false),
                    totDailyCalories = table.Column<int>(type: "int", nullable: true),
                    userId = table.Column<int>(type: "int", nullable: false),
                    creationDate = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nutrition", x => x.nutritionId);
                    table.ForeignKey(
                        name: "FK_nutrition_typeNutrition",
                        column: x => x.typeNutritionId,
                        principalTable: "typeNutrition",
                        principalColumn: "typeNutritionId");
                });

            migrationBuilder.CreateTable(
                name: "product",
                columns: table => new
                {
                    productId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    typePlanId = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    expirationDate = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product", x => x.productId);
                    table.ForeignKey(
                        name: "FK_product_typePlan",
                        column: x => x.typePlanId,
                        principalTable: "typePlan",
                        principalColumn: "typePlaneId");
                });

            migrationBuilder.CreateTable(
                name: "training",
                columns: table => new
                {
                    trainingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    typeId = table.Column<int>(type: "int", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: false),
                    creationDate = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true, defaultValueSql: "(getdate())"),
                    endDate = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    levelId = table.Column<int>(type: "int", nullable: true),
                    duration = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_training", x => x.trainingId);
                    table.ForeignKey(
                        name: "FK_training_levelTraining",
                        column: x => x.levelId,
                        principalTable: "levelTraining",
                        principalColumn: "levelId");
                    table.ForeignKey(
                        name: "FK_training_typeTrainig",
                        column: x => x.typeId,
                        principalTable: "typeTrainig",
                        principalColumn: "typeId");
                });

            migrationBuilder.CreateTable(
                name: "mealIngredient",
                columns: table => new
                {
                    mealId = table.Column<int>(type: "int", nullable: false),
                    ingredientId = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<double>(type: "float", nullable: false),
                    unit = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__mealIngr__6F15A6E8793153BC", x => new { x.mealId, x.ingredientId });
                    table.ForeignKey(
                        name: "FK__mealIngre__ingre__220B0B18",
                        column: x => x.ingredientId,
                        principalTable: "ingredient",
                        principalColumn: "ingredientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__mealIngre__mealI__2116E6DF",
                        column: x => x.mealId,
                        principalTable: "meal",
                        principalColumn: "mealId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "appointment",
                columns: table => new
                {
                    appointmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    date = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ptId = table.Column<int>(type: "int", nullable: false),
                    duration = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    userId = table.Column<int>(type: "int", nullable: false),
                    callUrl = table.Column<string>(type: "varchar(350)", unicode: false, maxLength: 350, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointment", x => x.appointmentId);
                    table.ForeignKey(
                        name: "FK_appointment_user",
                        column: x => x.userId,
                        principalTable: "user",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "measure",
                columns: table => new
                {
                    measureId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    bodyId = table.Column<int>(type: "int", nullable: false),
                    cm = table.Column<int>(type: "int", nullable: false),
                    collectPeriod = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValueSql: "(getdate())"),
                    userId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_measure", x => x.measureId);
                    table.ForeignKey(
                        name: "FK_measure_bodyPart",
                        column: x => x.bodyId,
                        principalTable: "bodyPart",
                        principalColumn: "bodyPartId");
                    table.ForeignKey(
                        name: "FK_measure_user",
                        column: x => x.userId,
                        principalTable: "user",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "responseQuiz",
                columns: table => new
                {
                    responseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    answerId = table.Column<int>(type: "int", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_responseQuiz", x => x.responseId);
                    table.ForeignKey(
                        name: "FK_responseQuiz_answer",
                        column: x => x.answerId,
                        principalTable: "answer",
                        principalColumn: "answerId");
                    table.ForeignKey(
                        name: "FK_responseQuiz_user",
                        column: x => x.userId,
                        principalTable: "user",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "nutritionMeal",
                columns: table => new
                {
                    nutritionId = table.Column<int>(type: "int", nullable: false),
                    mealId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_nutritionMeal_meal",
                        column: x => x.mealId,
                        principalTable: "meal",
                        principalColumn: "mealId");
                    table.ForeignKey(
                        name: "FK_nutritionMeal_nutrition",
                        column: x => x.nutritionId,
                        principalTable: "nutrition",
                        principalColumn: "nutritionId");
                });

            migrationBuilder.CreateTable(
                name: "order",
                columns: table => new
                {
                    orderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productId = table.Column<int>(type: "int", nullable: false),
                    paymentTypeId = table.Column<int>(type: "int", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    paymentDate = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order", x => x.orderId);
                    table.ForeignKey(
                        name: "FK_order_paymentType",
                        column: x => x.paymentTypeId,
                        principalTable: "paymentType",
                        principalColumn: "paymentTypeId");
                    table.ForeignKey(
                        name: "FK_order_product",
                        column: x => x.productId,
                        principalTable: "product",
                        principalColumn: "productId");
                    table.ForeignKey(
                        name: "FK_order_user",
                        column: x => x.userId,
                        principalTable: "user",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "detailExercise",
                columns: table => new
                {
                    detailExerciseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nRipetition = table.Column<int>(type: "int", nullable: true),
                    pause = table.Column<int>(type: "int", nullable: true),
                    phase = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    exerciseId = table.Column<int>(type: "int", nullable: false),
                    trainingId = table.Column<int>(type: "int", nullable: false),
                    series = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detailExercise", x => x.detailExerciseId);
                    table.ForeignKey(
                        name: "FK_detailExercise_exercise",
                        column: x => x.exerciseId,
                        principalTable: "exercise",
                        principalColumn: "exerciseId");
                    table.ForeignKey(
                        name: "FK_detailExercise_training",
                        column: x => x.trainingId,
                        principalTable: "training",
                        principalColumn: "trainingId");
                });

            migrationBuilder.CreateTable(
                name: "billing",
                columns: table => new
                {
                    billingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    paymentTypeId = table.Column<int>(type: "int", nullable: false),
                    orderId = table.Column<int>(type: "int", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    amount = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_billing", x => x.billingId);
                    table.ForeignKey(
                        name: "FK_billing_order",
                        column: x => x.orderId,
                        principalTable: "order",
                        principalColumn: "orderId");
                    table.ForeignKey(
                        name: "FK_billing_paymentType",
                        column: x => x.paymentTypeId,
                        principalTable: "paymentType",
                        principalColumn: "paymentTypeId");
                    table.ForeignKey(
                        name: "FK_billing_user",
                        column: x => x.userId,
                        principalTable: "user",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_answer_questionId",
                table: "answer",
                column: "questionId");

            migrationBuilder.CreateIndex(
                name: "IX_appointment_userId",
                table: "appointment",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_billing_orderId",
                table: "billing",
                column: "orderId");

            migrationBuilder.CreateIndex(
                name: "IX_billing_paymentTypeId",
                table: "billing",
                column: "paymentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_billing_userId",
                table: "billing",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_detailExercise_exerciseId",
                table: "detailExercise",
                column: "exerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_detailExercise_trainingId",
                table: "detailExercise",
                column: "trainingId");

            migrationBuilder.CreateIndex(
                name: "IX_exercise_locationTrainingId",
                table: "exercise",
                column: "locationTrainingId");

            migrationBuilder.CreateIndex(
                name: "IX_meal_categoryId",
                table: "meal",
                column: "categoryId");

            migrationBuilder.CreateIndex(
                name: "IX_mealIngredient_ingredientId",
                table: "mealIngredient",
                column: "ingredientId");

            migrationBuilder.CreateIndex(
                name: "IX_measure_bodyId",
                table: "measure",
                column: "bodyId");

            migrationBuilder.CreateIndex(
                name: "IX_measure_userId",
                table: "measure",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_nutrition_typeNutritionId",
                table: "nutrition",
                column: "typeNutritionId");

            migrationBuilder.CreateIndex(
                name: "IX_nutritionMeal_mealId",
                table: "nutritionMeal",
                column: "mealId");

            migrationBuilder.CreateIndex(
                name: "IX_nutritionMeal_nutritionId",
                table: "nutritionMeal",
                column: "nutritionId");

            migrationBuilder.CreateIndex(
                name: "IX_order_paymentTypeId",
                table: "order",
                column: "paymentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_order_productId",
                table: "order",
                column: "productId");

            migrationBuilder.CreateIndex(
                name: "IX_order_userId",
                table: "order",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_product_typePlanId",
                table: "product",
                column: "typePlanId");

            migrationBuilder.CreateIndex(
                name: "IX_quiz_inputTypeId",
                table: "quiz",
                column: "inputTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_quiz_questionId",
                table: "quiz",
                column: "questionId");

            migrationBuilder.CreateIndex(
                name: "IX_responseQuiz_answerId",
                table: "responseQuiz",
                column: "answerId");

            migrationBuilder.CreateIndex(
                name: "IX_responseQuiz_userId",
                table: "responseQuiz",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_training_levelId",
                table: "training",
                column: "levelId");

            migrationBuilder.CreateIndex(
                name: "IX_training_typeId",
                table: "training",
                column: "typeId");

            migrationBuilder.CreateIndex(
                name: "IX_user_roleId",
                table: "user",
                column: "roleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appointment");

            migrationBuilder.DropTable(
                name: "billing");

            migrationBuilder.DropTable(
                name: "detailExercise");

            migrationBuilder.DropTable(
                name: "mealIngredient");

            migrationBuilder.DropTable(
                name: "measure");

            migrationBuilder.DropTable(
                name: "nutritionMeal");

            migrationBuilder.DropTable(
                name: "quiz");

            migrationBuilder.DropTable(
                name: "responseQuiz");

            migrationBuilder.DropTable(
                name: "order");

            migrationBuilder.DropTable(
                name: "exercise");

            migrationBuilder.DropTable(
                name: "training");

            migrationBuilder.DropTable(
                name: "ingredient");

            migrationBuilder.DropTable(
                name: "bodyPart");

            migrationBuilder.DropTable(
                name: "meal");

            migrationBuilder.DropTable(
                name: "nutrition");

            migrationBuilder.DropTable(
                name: "inputType");

            migrationBuilder.DropTable(
                name: "answer");

            migrationBuilder.DropTable(
                name: "paymentType");

            migrationBuilder.DropTable(
                name: "product");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "locationTraining");

            migrationBuilder.DropTable(
                name: "levelTraining");

            migrationBuilder.DropTable(
                name: "typeTrainig");

            migrationBuilder.DropTable(
                name: "categoryOfDay");

            migrationBuilder.DropTable(
                name: "typeNutrition");

            migrationBuilder.DropTable(
                name: "question");

            migrationBuilder.DropTable(
                name: "typePlan");

            migrationBuilder.DropTable(
                name: "roleUser");
        }
    }
}
