using System.Text.Json.Serialization;

namespace slowfit.DTORequest
{
    public class DetailExerciseRes
    {
        public int DetailExerciseId { get; set; }

        public int? NRipetition { get; set; }

        public int? Pause { get; set; }

        public string? Phase { get; set; }

        public int? Series { get; set; }

        public decimal? Kg { get; set; }

        public int ExerciseId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int TrainingId { get; set; }
    }
}
