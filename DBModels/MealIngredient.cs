using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class MealIngredient
{
    public int MealIngredientId { get; set; }

    public int IngredientId { get; set; }

    public int MealId { get; set; }

    public int Quantity { get; set; }

    public string Unit { get; set; } = null!;

    public virtual Ingredient Ingredient { get; set; } = null!;

    public virtual Meal Meal { get; set; } = null!;
}
