using slowfit.DTORequest;

namespace slowfit.DTOResponse
{
    public class QuizUserResponse
    {
        
            public int QuizId { get; set; }

            public int QuestionId { get; set; }

            public int? InputTypeId { get; set; }

            public bool? Input { get; set; }

            public bool SingleResponse { get; set; }

            public string? Type { get; set; } = null;

            public string? QuestionText { get; set; } = string.Empty;

            public List<AnswerRes>? Answers { get; set; }
        }

}
