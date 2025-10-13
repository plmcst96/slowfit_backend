using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class Answer
{
    public int AnswerId { get; set; }

    public string AnswerString { get; set; } = null!;

    public int QuestionId { get; set; }

    public virtual Question Question { get; set; } = null!;

    public virtual ICollection<ResponseQuiz> ResponseQuizzes { get; set; } = new List<ResponseQuiz>();
}
