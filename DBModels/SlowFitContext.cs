using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace slowfit.DBModels;

public partial class SlowFitContext : DbContext
{
    public SlowFitContext()
    {
    }

    public SlowFitContext(DbContextOptions<SlowFitContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Answer> Answers { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Billing> Billings { get; set; }

    public virtual DbSet<BodyPart> BodyParts { get; set; }

    public virtual DbSet<CategoryOfDay> CategoryOfDays { get; set; }

    public virtual DbSet<DayWeek> DayWeeks { get; set; }

    public virtual DbSet<DetailExercise> DetailExercises { get; set; }

    public virtual DbSet<Exercise> Exercises { get; set; }

    public virtual DbSet<Ingredient> Ingredients { get; set; }

    public virtual DbSet<InputType> InputTypes { get; set; }

    public virtual DbSet<LevelTraining> LevelTrainings { get; set; }

    public virtual DbSet<LocationTraining> LocationTrainings { get; set; }

    public virtual DbSet<Meal> Meals { get; set; }

    public virtual DbSet<MealIngredient> MealIngredients { get; set; }

    public virtual DbSet<Measure> Measures { get; set; }

    public virtual DbSet<NotificationsFire> NotificationsFires { get; set; }

    public virtual DbSet<Nutrition> Nutritions { get; set; }

    public virtual DbSet<NutritionMeal> NutritionMeals { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<PaymentType> PaymentTypes { get; set; }

    public virtual DbSet<PersonalTrainer> PersonalTrainers { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProgressNutrition> ProgressNutritions { get; set; }

    public virtual DbSet<ProgressTraining> ProgressTrainings { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Quiz> Quizzes { get; set; }

    public virtual DbSet<ResponseQuiz> ResponseQuizzes { get; set; }

    public virtual DbSet<RoleUser> RoleUsers { get; set; }

    public virtual DbSet<Training> Training { get; set; }

    public virtual DbSet<TypeNutrition> TypeNutritions { get; set; }

    public virtual DbSet<TypePlan> TypePlans { get; set; }

    public virtual DbSet<TypeTrainig> TypeTrainigs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__SlowFitDb");
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.ToTable("answer");

            entity.HasIndex(e => e.QuestionId, "IX_answer_questionId");

            entity.Property(e => e.AnswerId).HasColumnName("answerId");
            entity.Property(e => e.AnswerString)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("answerString");
            entity.Property(e => e.QuestionId).HasColumnName("questionId");

            entity.HasOne(d => d.Question).WithMany(p => p.Answers)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_answer_question");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.ToTable("appointment");

            entity.HasIndex(e => e.UserId, "IX_appointment_userId");

            entity.Property(e => e.AppointmentId).HasColumnName("appointmentId");
            entity.Property(e => e.CallUrl)
                .HasMaxLength(350)
                .IsUnicode(false)
                .HasColumnName("callUrl");
            entity.Property(e => e.Date)
                .HasColumnType("datetime2")
                .HasColumnName("date");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.PtId).HasColumnName("ptId");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.User).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_appointment_user");

            entity.HasOne(d => d.Pt).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PtId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_appointment_personalTrainer");
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.Property(e => e.CreatedTime).HasColumnName("Created_time");
            entity.Property(e => e.DeletedTime).HasColumnName("Deleted_time");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UpdatedTime).HasColumnName("Updated_time");
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Billing>(entity =>
        {
            entity.ToTable("billing");

            entity.HasIndex(e => e.OrderId, "IX_billing_orderId");

            entity.HasIndex(e => e.PaymentTypeId, "IX_billing_paymentTypeId");

            entity.HasIndex(e => e.UserId, "IX_billing_userId");

            entity.Property(e => e.BillingId).HasColumnName("billingId");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.Date)
                .HasColumnType("date")
                .HasColumnName("date");
            entity.Property(e => e.OrderId).HasColumnName("orderId");
            entity.Property(e => e.PaymentTypeId).HasColumnName("paymentTypeId");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Order).WithMany(p => p.Billings)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_billing_order");

            entity.HasOne(d => d.PaymentType).WithMany(p => p.Billings)
                .HasForeignKey(d => d.PaymentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_billing_paymentType");

            entity.HasOne(d => d.User).WithMany(p => p.Billings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_billing_user");
        });

        modelBuilder.Entity<BodyPart>(entity =>
        {
            entity.ToTable("bodyPart");

            entity.Property(e => e.BodyPartId).HasColumnName("bodyPartId");
            entity.Property(e => e.BodyPartName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("bodyPartName");
        });

        modelBuilder.Entity<CategoryOfDay>(entity =>
        {
            entity.HasKey(e => e.CategoryId);

            entity.ToTable("categoryOfDay");

            entity.Property(e => e.CategoryId).HasColumnName("categoryId");
            entity.Property(e => e.MomentOfDay)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("momentOfDay");
        });

        modelBuilder.Entity<DayWeek>(entity =>
        {
            entity.HasKey(e => e.DayId);

            entity.ToTable("dayWeek");

            entity.Property(e => e.DayId).HasColumnName("dayId");
            entity.Property(e => e.DayString)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("dayString");
        });

        modelBuilder.Entity<DetailExercise>(entity =>
        {
            entity.ToTable("detailExercise");

            entity.HasIndex(e => e.ExerciseId, "IX_detailExercise_exerciseId");

            entity.HasIndex(e => e.TrainingId, "IX_detailExercise_trainingId");

            entity.Property(e => e.DetailExerciseId).HasColumnName("detailExerciseId");
            entity.Property(e => e.ExerciseId).HasColumnName("exerciseId");
            entity.Property(e => e.NRipetition).HasColumnName("nRipetition");
            entity.Property(e => e.Pause).HasColumnName("pause");
            entity.Property(e => e.Kg)
                .HasColumnType("decimal(5, 0)")
                .HasColumnName("kg");
            entity.Property(e => e.Phase)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("phase");
            entity.Property(e => e.Series).HasColumnName("series");
            entity.Property(e => e.TrainingId).HasColumnName("trainingId");

            entity.HasOne(d => d.Exercise).WithMany(p => p.DetailExercises)
                .HasForeignKey(d => d.ExerciseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_detailExercise_exercise");

            entity.HasOne(d => d.Training).WithMany(p => p.DetailExercises)
                .HasForeignKey(d => d.TrainingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_detailExercise_training");
        });

        modelBuilder.Entity<Exercise>(entity =>
        {
            entity.ToTable("exercise");

            entity.HasIndex(e => e.LocationTrainingId, "IX_exercise_locationTrainingId");

            entity.Property(e => e.ExerciseId).HasColumnName("exerciseId");
            entity.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Image)
                .IsUnicode(false)
                .HasColumnName("image");
            entity.Property(e => e.LocationTrainingId).HasColumnName("locationTrainingId");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.TypeTrainingId).HasColumnName("typeTrainingId");
            entity.Property(e => e.UrlVideo)
                .IsUnicode(false)
                .HasColumnName("urlVideo");

            entity.HasOne(d => d.LocationTraining).WithMany(p => p.Exercises)
                .HasForeignKey(d => d.LocationTrainingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_exercise_locationTraining");
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.ToTable("ingredient");

            entity.Property(e => e.IngredientId).HasColumnName("ingredientId");
            entity.Property(e => e.Calories).HasColumnName("calories");
            entity.Property(e => e.Carbohydrate)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("carbohydrate");
            entity.Property(e => e.Fats)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("fats");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Protein)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("protein");
        });

        modelBuilder.Entity<InputType>(entity =>
        {
            entity.ToTable("inputType");

            entity.Property(e => e.InputTypeId).HasColumnName("inputTypeId");
            entity.Property(e => e.InputTypeName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("inputTypeName");
        });

        modelBuilder.Entity<LevelTraining>(entity =>
        {
            entity.HasKey(e => e.LevelId);

            entity.ToTable("levelTraining");

            entity.Property(e => e.LevelId).HasColumnName("levelId");
            entity.Property(e => e.LevelString)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("levelString");
        });

        modelBuilder.Entity<LocationTraining>(entity =>
        {
            entity.HasKey(e => e.LocationId);

            entity.ToTable("locationTraining");

            entity.Property(e => e.LocationId).HasColumnName("locationId");
            entity.Property(e => e.LocationString)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("locationString");
        });

        modelBuilder.Entity<Meal>(entity =>
        {
            entity.ToTable("meal");

            entity.HasIndex(e => e.CategoryId, "IX_meal_categoryId");

            entity.Property(e => e.MealId).HasColumnName("mealId");
            entity.Property(e => e.Calories).HasColumnName("calories");
            entity.Property(e => e.Carbohydrate).HasColumnName("carbohydrate");
            entity.Property(e => e.CategoryId).HasColumnName("categoryId");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Difficulty).HasColumnName("difficulty");
            entity.Property(e => e.Fats).HasColumnName("fats");
            entity.Property(e => e.ImageMeal)
                .IsUnicode(false)
                .HasColumnName("imageMeal");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.PreparingTime).HasColumnName("preparingTime");
            entity.Property(e => e.Protein).HasColumnName("protein");
            entity.Property(e => e.Recipe)
                .IsUnicode(false)
                .HasColumnName("recipe");

            entity.HasOne(d => d.Category).WithMany(p => p.Meals)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_meal_categoryOfDay");
        });

        modelBuilder.Entity<MealIngredient>(entity =>
        {
            entity.HasKey(e => e.MealIngredientId).HasName("PK_Table_1");

            entity.ToTable("mealIngredient");

            entity.Property(e => e.MealIngredientId).HasColumnName("mealIngredientId");
            entity.Property(e => e.IngredientId).HasColumnName("ingredientId");
            entity.Property(e => e.MealId).HasColumnName("mealId");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("unit");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.MealIngredients)
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_mealIngredient_ingredient");

            entity.HasOne(d => d.Meal).WithMany(p => p.MealIngredients)
                .HasForeignKey(d => d.MealId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_mealIngredient_meal");
        });

        modelBuilder.Entity<Measure>(entity =>
        {
            entity.ToTable("measure");

            entity.HasIndex(e => e.BodyId, "IX_measure_bodyId");

            entity.HasIndex(e => e.UserId, "IX_measure_userId");

            entity.Property(e => e.MeasureId).HasColumnName("measureId");
            entity.Property(e => e.BodyId).HasColumnName("bodyId");
            entity.Property(e => e.Cm).HasColumnName("cm");
            entity.Property(e => e.CollectPeriod)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("date")
                .HasColumnName("collectPeriod");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Body).WithMany(p => p.Measures)
                .HasForeignKey(d => d.BodyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_measure_bodyPart");

            entity.HasOne(d => d.User).WithMany(p => p.Measures)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_measure_user");
        });

        modelBuilder.Entity<NotificationsFire>(entity =>
        {
            entity.ToTable("NotificationsFire");

            entity.Property(e => e.Body).IsRequired();
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ReceiverRole).HasMaxLength(20);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Receiver).WithMany(p => p.NotificationsFires)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NotificationsFire_User");
        });

        modelBuilder.Entity<Nutrition>(entity =>
        {
            entity.ToTable("nutrition");

            entity.Property(e => e.NutritionId).HasColumnName("nutritionId");
            entity.Property(e => e.CreationDate)
                .HasColumnType("date")
                .HasColumnName("creationDate");
            entity.Property(e => e.ExpirationDate)
                .HasColumnType("date")
                .HasColumnName("expirationDate");
            entity.Property(e => e.TotDailyCalories).HasColumnName("totDailyCalories");
            entity.Property(e => e.TypeNutritionId).HasColumnName("typeNutritionId");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.Property(e => e.PtId).HasColumnName("ptId");

            entity.HasOne(d => d.TypeNutrition).WithMany(p => p.Nutritions)
                .HasForeignKey(d => d.TypeNutritionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_nutrition_typeNutrition");

            entity.HasOne(d => d.User).WithMany(p => p.Nutritions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_nutrition_user");

            entity.HasOne(d => d.Pt).WithMany(p => p.Nutritions)
                .HasForeignKey(d => d.PtId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_nutrition_personalTrainer");
        });

        modelBuilder.Entity<NutritionMeal>(entity =>
        {
            entity.HasKey(e => new { e.NutritionId, e.MealId, e.DayId }).HasName("PK_NutritionMeals");

            entity.ToTable("nutritionMeal");

            entity.HasIndex(e => e.MealId, "IX_nutritionMeal_mealId");

            entity.HasIndex(e => e.NutritionId, "IX_nutritionMeal_nutritionId");

            entity.Property(e => e.NutritionId).HasColumnName("nutritionId");
            entity.Property(e => e.MealId).HasColumnName("mealId");
            entity.Property(e => e.DayId).HasColumnName("dayId");

            entity.HasOne(d => d.Day).WithMany(p => p.NutritionMeals)
                .HasForeignKey(d => d.DayId)
                .HasConstraintName("FK_nutritionMeal_dayWeek");

            entity.HasOne(d => d.Meal).WithMany(p => p.NutritionMeals)
                .HasForeignKey(d => d.MealId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_nutritionMeal_meal");

            entity.HasOne(d => d.Nutrition).WithMany(p => p.NutritionMeals)
                .HasForeignKey(d => d.NutritionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_nutritionMeal_nutrition");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("order");

            entity.HasIndex(e => e.PaymentTypeId, "IX_order_paymentTypeId");

            entity.HasIndex(e => e.ProductId, "IX_order_productId");

            entity.HasIndex(e => e.UserId, "IX_order_userId");

            entity.Property(e => e.OrderId).HasColumnName("orderId");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.PaymentDate)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("paymentDate");
            entity.Property(e => e.PaymentTypeId).HasColumnName("paymentTypeId");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.PaymentType).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_paymentType");

            entity.HasOne(d => d.Product).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_product");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_user");
        });

        modelBuilder.Entity<PaymentType>(entity =>
        {
            entity.ToTable("paymentType");

            entity.Property(e => e.PaymentTypeId).HasColumnName("paymentTypeId");
            entity.Property(e => e.PaymentTypeName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("paymentTypeName");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("product");

            entity.HasIndex(e => e.TypePlanId, "IX_product_typePlanId");

            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.ExpirationDate)
                .HasColumnType("date")
                .HasColumnName("expirationDate");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("price");
            entity.Property(e => e.TypePlanId).HasColumnName("typePlanId");

            entity.HasOne(d => d.TypePlan).WithMany(p => p.Products)
                .HasForeignKey(d => d.TypePlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_product_typePlan");
        });

        modelBuilder.Entity<ProgressNutrition>(entity =>
        {
            entity.ToTable("progressNutrition");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AvarageKcal).HasColumnName("avarageKcal");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime2")
                .HasColumnName("createdAt");
            entity.Property(e => e.DateOfProgress)
                .HasColumnType("date")
                .HasColumnName("dateOfProgress");
            entity.Property(e => e.NutritionId).HasColumnName("nutritionId");
            entity.Property(e => e.ProgressValue).HasColumnName("progressValue");
            entity.Property(e => e.UserId).HasColumnName("userId");
        });

        modelBuilder.Entity<ProgressTraining>(entity =>
        {
            entity.ToTable("progressTraining");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AvarageKg).HasColumnName("avarageKg");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime2")
                .HasColumnName("createdAt");
            entity.Property(e => e.DateOfProgress)
                .HasColumnType("date")
                .HasColumnName("dateOfProgress");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.ProgressValue).HasColumnName("progressValue");
            entity.Property(e => e.TrainingId).HasColumnName("trainingId");
            entity.Property(e => e.UserId).HasColumnName("userId");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.ToTable("question");

            entity.Property(e => e.QuestionId).HasColumnName("questionId");
            entity.Property(e => e.QuestionString)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("questionString");
        });

        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.ToTable("quiz");

            entity.HasIndex(e => e.InputTypeId, "IX_quiz_inputTypeId");

            entity.HasIndex(e => e.QuestionId, "IX_quiz_questionId");

            entity.Property(e => e.QuizId).HasColumnName("quizId");
            entity.Property(e => e.Input).HasColumnName("input");
            entity.Property(e => e.InputTypeId).HasColumnName("inputTypeId");
            entity.Property(e => e.QuestionId).HasColumnName("questionId");
            entity.Property(e => e.SingleResponse).HasColumnName("singleResponse");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type");

            entity.HasOne(d => d.InputType).WithMany(p => p.Quizzes)
                .HasForeignKey(d => d.InputTypeId)
                .HasConstraintName("FK_quiz_inputType");

            entity.HasOne(d => d.Question).WithMany(p => p.Quizzes)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_quiz_question");
        });

        modelBuilder.Entity<ResponseQuiz>(entity =>
        {
            entity.HasKey(e => e.ResponseId);

            entity.ToTable("responseQuiz");

            entity.HasIndex(e => e.AnswerId, "IX_responseQuiz_answerId");

            entity.HasIndex(e => e.UserId, "IX_responseQuiz_userId");

            entity.Property(e => e.ResponseId).HasColumnName("responseId");
            entity.Property(e => e.AnswerId).HasColumnName("answerId");
            entity.Property(e => e.AnswerString)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("answerString");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Answer).WithMany(p => p.ResponseQuizzes)
                .HasForeignKey(d => d.AnswerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_responseQuiz_answer");

            entity.HasOne(d => d.User).WithMany(p => p.ResponseQuizzes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_responseQuiz_user");
        });

        modelBuilder.Entity<RoleUser>(entity =>
        {
            entity.HasKey(e => e.RoleId);

            entity.ToTable("roleUser");

            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("roleName");
        });

        modelBuilder.Entity<Training>(entity =>
        {
            entity.ToTable("training");

            entity.HasIndex(e => e.LevelId, "IX_training_levelId");

            entity.HasIndex(e => e.TypeId, "IX_training_typeId");

            entity.Property(e => e.TrainingId).HasColumnName("trainingId");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("date")
                .HasColumnName("creationDate");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.EndDate)
                .HasColumnType("date")
                .HasColumnName("endDate");
            entity.Property(e => e.LevelId).HasColumnName("levelId");
            entity.Property(e => e.TypeId).HasColumnName("typeId");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.Property(e => e.PtId).HasColumnName("ptId");

            entity.HasOne(d => d.Level).WithMany(p => p.Training)
                .HasForeignKey(d => d.LevelId)
                .HasConstraintName("FK_training_levelTraining");

            entity.HasOne(d => d.Type).WithMany(p => p.Training)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_training_typeTrainig");

            entity.HasOne(d => d.Pt).WithMany(p => p.Trainings)
                .HasForeignKey(d => d.PtId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_training_personalTrainer");
        });

        modelBuilder.Entity<TypeNutrition>(entity =>
        {
            entity.ToTable("typeNutrition");

            entity.Property(e => e.TypeNutritionId).HasColumnName("typeNutritionId");
            entity.Property(e => e.TypeNutritionName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("typeNutritionName");
        });

        modelBuilder.Entity<TypePlan>(entity =>
        {
            entity.HasKey(e => e.TypePlaneId);

            entity.ToTable("typePlan");

            entity.Property(e => e.TypePlaneId).HasColumnName("typePlaneId");
            entity.Property(e => e.TypePlaneName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("typePlaneName");
        });

        modelBuilder.Entity<TypeTrainig>(entity =>
        {
            entity.HasKey(e => e.TypeId);

            entity.ToTable("typeTrainig");

            entity.Property(e => e.TypeId).HasColumnName("typeId");
            entity.Property(e => e.TypeName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("typeName");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user");

            entity.HasIndex(e => e.RoleId, "IX_user_roleId");

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.Address)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.BirthDate)
                .HasColumnType("date")
                .HasColumnName("birthDate");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("country");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.AccessFailedCount).HasColumnName("AccessFailedCount");
            entity.Property(e => e.ConcurrencyStamp).HasColumnName("ConcurrencyStamp");
            entity.Property(e => e.FcmToken).HasColumnName("FcmToken");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("firstName");
            entity.Property(e => e.ImageProfile)
                .IsUnicode(false)
                .HasColumnName("imageProfile");
            entity.Property(e => e.LockoutEnabled).HasColumnName("LockoutEnabled");
            entity.Property(e => e.LockoutEnd).HasColumnName("LockoutEnd");
            entity.Property(e => e.Password)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.PasswordHash).HasColumnName("PasswordHash");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.Province)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("province");
            entity.Property(e => e.PtId).HasColumnName("ptId");
            entity.Property(e => e.RefreshTokenExpiresAt)
                .HasColumnType("datetime2")
                .HasColumnName("refreshTokenExpiresAt");
            entity.Property(e => e.RefreshTokenHash)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("refreshTokenHash");
            entity.Property(e => e.RefreshTokenRevokedAt)
                .HasColumnType("datetime2")
                .HasColumnName("refreshTokenRevokedAt");
            entity.Property(e => e.RoleId)
                .HasDefaultValue(1)
                .HasColumnName("roleId");
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("surname");
            entity.Property(e => e.SecurityStamp).HasColumnName("SecurityStamp");
            entity.Property(e => e.ZipCode).HasColumnName("zipCode");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_user_roleUser");

            entity.HasOne(d => d.Pt).WithMany(p => p.Clients)
                .HasForeignKey(d => d.PtId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_user_personalTrainer");
        });

        modelBuilder.Entity<PersonalTrainer>(entity =>
        {
            entity.HasKey(e => e.PtId);

            entity.ToTable("personalTrainer");

            entity.Property(e => e.PtId).HasColumnName("ptId");
            entity.Property(e => e.Address)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.BirthDate)
                .HasColumnType("date")
                .HasColumnName("birthDate");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("country");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FcmToken).HasColumnName("FcmToken");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("firstName");
            entity.Property(e => e.FiscalCode)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("fiscalCode");
            entity.Property(e => e.ImageProfile)
                .IsUnicode(false)
                .HasColumnName("imageProfile");
            entity.Property(e => e.Password)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.PasswordSetupTokenExpiresAt)
                .HasColumnType("datetime2")
                .HasColumnName("passwordSetupTokenExpiresAt");
            entity.Property(e => e.PasswordSetupTokenHash)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("passwordSetupTokenHash");
            entity.Property(e => e.PecEmail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("pecEmail");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.Province)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("province");
            entity.Property(e => e.RefreshTokenExpiresAt)
                .HasColumnType("datetime2")
                .HasColumnName("refreshTokenExpiresAt");
            entity.Property(e => e.RefreshTokenHash)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("refreshTokenHash");
            entity.Property(e => e.RefreshTokenRevokedAt)
                .HasColumnType("datetime2")
                .HasColumnName("refreshTokenRevokedAt");
            entity.Property(e => e.SdiCode)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("sdiCode");
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("surname");
            entity.Property(e => e.VatNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("vatNumber");
            entity.Property(e => e.ZipCode).HasColumnName("zipCode");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
