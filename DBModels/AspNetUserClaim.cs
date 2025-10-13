using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class AspNetUserClaim
{
    public int Id { get; set; }

    public long UserId { get; set; }

    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }

    public virtual AspNetUser User { get; set; } = null!;
}
