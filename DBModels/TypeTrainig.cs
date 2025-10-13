using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class TypeTrainig
{
    public int TypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Training> Training { get; set; } = new List<Training>();
}
