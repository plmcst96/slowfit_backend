using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class ResponseQuiz
{
    public int ResponseId { get; set; }

    public int AnswerId { get; set; }

    public int UserId { get; set; }

    public virtual Answer Answer { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
