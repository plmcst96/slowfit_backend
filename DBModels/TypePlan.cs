using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class TypePlan
{
    public int TypePlaneId { get; set; }

    public string TypePlaneName { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
