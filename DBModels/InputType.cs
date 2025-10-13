using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class InputType
{
    public int InputTypeId { get; set; }

    public string InputTypeName { get; set; } = null!;

    public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
}
