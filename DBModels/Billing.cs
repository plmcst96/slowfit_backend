using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class Billing
{
    public int BillingId { get; set; }

    public int PaymentTypeId { get; set; }

    public int OrderId { get; set; }

    public int UserId { get; set; }

    public DateTime Date { get; set; }

    public decimal Amount { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual PaymentType PaymentType { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
