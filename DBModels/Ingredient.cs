using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class Ingredient
{
    public int IngredientId { get; set; }

    public string Name { get; set; } = null!;

    public int? Calories { get; set; }

    public decimal? Protein { get; set; }

    public decimal? Fats { get; set; }

    public decimal? Carbohydrate { get; set; }

    public virtual ICollection<MealIngredient> MealIngredients { get; set; } = new List<MealIngredient>();
}
