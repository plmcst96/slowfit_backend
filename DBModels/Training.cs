using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class Training
{
    public int TrainingId { get; set; }

    public int TypeId { get; set; }

    public int UserId { get; set; }

    public string? CreationDate { get; set; }

    public string? EndDate { get; set; }

    public int? LevelId { get; set; }

    public int? Duration { get; set; }

    public virtual ICollection<DetailExercise> DetailExercises { get; set; } = new List<DetailExercise>();

    public virtual LevelTraining? Level { get; set; }

    public virtual TypeTrainig Type { get; set; } = null!;
}
