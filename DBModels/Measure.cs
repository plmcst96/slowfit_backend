using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class Measure
{
    public int MeasureId { get; set; }

    public int BodyId { get; set; }

    public int Cm { get; set; }

    public DateTime CollectPeriod { get; set; }

    public int UserId { get; set; }

    public virtual BodyPart Body { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
