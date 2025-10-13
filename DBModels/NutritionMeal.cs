using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class NutritionMeal
{
    public int NutritionId { get; set; }

    public int MealId { get; set; }

    public int? DayId { get; set; }

    public virtual DayWeek? Day { get; set; }

    public virtual Meal Meal { get; set; } = null!;

    public virtual Nutrition Nutrition { get; set; } = null!;
}
