using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class DayWeek
{
    public int DayId { get; set; }

    public string DayString { get; set; } = null!;

    public virtual ICollection<NutritionMeal> NutritionMeals { get; set; } = new List<NutritionMeal>();
}
