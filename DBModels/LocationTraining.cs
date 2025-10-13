using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class LocationTraining
{
    public int LocationId { get; set; }

    public string LocationString { get; set; } = null!;

    public virtual ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
}
