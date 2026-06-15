using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class Nutrition
{
    public int NutritionId { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public int TypeNutritionId { get; set; }

    public int? TotDailyCalories { get; set; }

    public int? UserId { get; set; }

    public DateTime? CreationDate { get; set; }

    public virtual ICollection<NutritionMeal> NutritionMeals { get; set; } = new List<NutritionMeal>();

    public virtual TypeNutrition TypeNutrition { get; set; } = null!;

    public virtual User? User { get; set; }
}
