using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public DateTime Date { get; set; }

    public int PtId { get; set; }

    public int Duration { get; set; }

    public string? Description { get; set; }

    public int UserId { get; set; }

    public string? CallUrl { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual PersonalTrainer Pt { get; set; } = null!;
}
