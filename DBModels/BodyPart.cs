using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class BodyPart
{
    public int BodyPartId { get; set; }

    public string BodyPartName { get; set; } = null!;

    public virtual ICollection<Measure> Measures { get; set; } = new List<Measure>();
}
