namespace slowfit.DTORequest
{
    public class ExerciseRes
    {
        public int ExerciseId { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string UrlVideo { get; set; } = null!;

        public string? Image { get; set; }

        public int TypeTrainingId { get; set; }

        public int LocationTrainingId { get; set; }
    }
}
