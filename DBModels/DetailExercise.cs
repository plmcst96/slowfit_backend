using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class DetailExercise
{
    public int DetailExerciseId { get; set; }

    public int? NRipetition { get; set; }

    public int? Pause { get; set; }

    public string? Phase { get; set; }

    public int ExerciseId { get; set; }

    public int TrainingId { get; set; }

    public int? Series { get; set; }

    public virtual Exercise Exercise { get; set; } = null!;

    public virtual Training Training { get; set; } = null!;
}
