using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class NotificationsFire
{
    public int Id { get; set; }

    public int ReceiverId { get; set; }

    public string ReceiverRole { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Body { get; set; } = null!;

    public string? Data { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User Receiver { get; set; } = null!;
}
