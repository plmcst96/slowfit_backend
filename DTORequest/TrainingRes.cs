using System.Text.Json.Serialization;

namespace slowfit.DTORequest
{
    public class TrainingRes
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int TrainingId { get; set; } // sarà ignorato in POST

        public int TypeId { get; set; }

        public int UserId { get; set; }

        public int? LevelId { get; set; }

        public int? Duration { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime EndDate { get; set; }

        public List<DetailExerciseRes>? DetailExercises { get; set; }
    }
}
