namespace slowfit.DTORequest
{
    public class NutritionResPost
    {
        public int NutritionId { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public List<NutritionMealReq> Meals { get; set; } = new();

        public int TypeNutritionId { get; set; }

        public int? TotDailyCalories { get; set; }

        public int? UserId { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
