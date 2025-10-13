using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class TypeNutrition
{
    public int TypeNutritionId { get; set; }

    public string TypeNutritionName { get; set; } = null!;

    public virtual ICollection<Nutrition> Nutritions { get; set; } = new List<Nutrition>();
}
