namespace slowfit.DTORequest
{
    public class IngredientRes
    {
        public int IngredientId { get; set; }

        public string Name { get; set; } = null!;

        public int? Calories { get; set; }

        public decimal? Protein { get; set; }

        public decimal? Fats { get; set; }

        public decimal? Carbohydrate { get; set; }

        public int Quantity { get; set; }

        public string? Unit { get; set; }
    }
}
