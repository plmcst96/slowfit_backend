using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public int TypePlanId { get; set; }

    public decimal Price { get; set; }

    public DateTime ExpirationDate { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual TypePlan TypePlan { get; set; } = null!;
}
