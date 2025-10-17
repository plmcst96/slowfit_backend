using slowfit.DBModels;

namespace slowfit.DTORequest
{
    public class QuizUserRes
    {
        public int QuizId { get; set; }

        public int QuestionId { get; set; }

        public int? InputTypeId { get; set; }

        public bool? Input { get; set; }

        public bool SingleResponse { get; set; }

        public string? Type { get; set; } = null;    }
}
