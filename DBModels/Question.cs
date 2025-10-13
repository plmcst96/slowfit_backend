using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class Question
{
    public int QuestionId { get; set; }

    public string QuestionString { get; set; } = null!;

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
}
