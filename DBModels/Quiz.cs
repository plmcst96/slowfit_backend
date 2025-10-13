using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class Quiz
{
    public int QuizId { get; set; }

    public int QuestionId { get; set; }

    public int? InputTypeId { get; set; }

    public bool? Input { get; set; }

    public bool SingleResponse { get; set; }

    public virtual InputType? InputType { get; set; }

    public virtual Question Question { get; set; } = null!;
}
