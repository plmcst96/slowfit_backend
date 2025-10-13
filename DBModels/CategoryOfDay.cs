using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class CategoryOfDay
{
    public int CategoryId { get; set; }

    public string MomentOfDay { get; set; } = null!;

    public virtual ICollection<Meal> Meals { get; set; } = new List<Meal>();
}
