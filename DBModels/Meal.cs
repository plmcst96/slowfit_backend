using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class Meal
{
    public int MealId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Recipe { get; set; } = null!;

    public int Calories { get; set; }

    public int Protein { get; set; }

    public int Fats { get; set; }

    public int Carbohydrate { get; set; }

    public int PreparingTime { get; set; }

    public string? ImageMeal { get; set; }

    public int? Difficulty { get; set; }

    public int? CategoryId { get; set; }

    public virtual CategoryOfDay? Category { get; set; }

    public virtual ICollection<MealIngredient> MealIngredients { get; set; } = [];

    public virtual ICollection<NutritionMeal> NutritionMeals { get; set; } = [];
}
