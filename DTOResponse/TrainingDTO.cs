
using System.Text.Json.Serialization;
using slowfit.DTOResponse;

namespace slowfit.DTOResponse
{
    public class TrainingDTO
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int TrainingId { get; set; }

        public int TypeId { get; set; }

        public int UserId { get; set; }

        public int? LevelId { get; set; }

        public int? Duration { get; set; }

        public string CreationDate { get; set; }

        public string EndDate { get; set; }

        public List<DetailExerciseDTO> DetailExercises { get; set; } = new();
    }

}


