using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class Exercise
{
    public int ExerciseId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? UrlVideo { get; set; }

    public string? Image { get; set; }

    public int TypeTrainingId { get; set; }

    public int LocationTrainingId { get; set; }

    public virtual ICollection<DetailExercise> DetailExercises { get; set; } = new List<DetailExercise>();

    public virtual LocationTraining LocationTraining { get; set; } = null!;
}
