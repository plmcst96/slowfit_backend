using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class LevelTraining
{
    public int LevelId { get; set; }

    public string LevelString { get; set; } = null!;

    public virtual ICollection<Training> Training { get; set; } = new List<Training>();
}
