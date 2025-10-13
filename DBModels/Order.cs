using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class Order
{
    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int PaymentTypeId { get; set; }

    public int UserId { get; set; }

    public decimal Amount { get; set; }

    public string? PaymentDate { get; set; }

    public virtual ICollection<Billing> Billings { get; set; } = new List<Billing>();

    public virtual PaymentType PaymentType { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
