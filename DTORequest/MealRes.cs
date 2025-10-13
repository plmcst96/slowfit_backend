
namespace slowfit.DTORequest
{
    public class MealRes
    {
        public int MealId { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Recipe { get; set; } = null!;

        public int IngredientId { get; set; }

        public int Calories { get; set; }

        public int Protein { get; set; }

        public int Fats { get; set; }

        public int Carbohydrate { get; set; }

        public int PreparingTime { get; set; }

        public string? ImageMeal { get; set; }

        public int? Difficulty { get; set; }

        public int? CategoryId { get; set; }

        public List<IngredientRes>? Ingredients { get; set; }
        public int? DayId { get; internal set; }
    }
}
