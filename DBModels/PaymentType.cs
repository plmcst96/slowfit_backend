using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class PaymentType
{
    public int PaymentTypeId { get; set; }

    public string PaymentTypeName { get; set; } = null!;

    public virtual ICollection<Billing> Billings { get; set; } = new List<Billing>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
