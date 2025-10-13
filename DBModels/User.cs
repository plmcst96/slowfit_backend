using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class User
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public string? Province { get; set; }

    public int? ZipCode { get; set; }

    public int? RoleId { get; set; }

    public string? BirthDate { get; set; }

    public int? PtId { get; set; }

    public string? ImageProfile { get; set; }

    public string? Phone { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Billing> Billings { get; set; } = new List<Billing>();

    public virtual ICollection<Measure> Measures { get; set; } = new List<Measure>();

    public virtual ICollection<Nutrition> Nutritions { get; set; } = new List<Nutrition>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<ResponseQuiz> ResponseQuizzes { get; set; } = new List<ResponseQuiz>();

    public virtual RoleUser? Role { get; set; }
}
