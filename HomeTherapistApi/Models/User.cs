using System;
using System.Collections.Generic;

namespace HomeTherapistApi.Models;

public partial class User
{
  public Guid Id { get; set; }

  public string? Username { get; set; }

  public string? NormalizedUsername { get; set; }

  public string? Email { get; set; }

  public string? NormalizedEmail { get; set; }

  public bool EmailConfirmed { get; set; }

  public string StaffId { get; set; } = null!;

  public uint? CertificateNumber { get; set; }

  public string? Address { get; set; }

  public decimal? Latitude { get; set; }

  public decimal? Longitude { get; set; }

  public uint? Radius { get; set; }

  public string? PasswordHash { get; set; }

  public string? SecurityStamp { get; set; }

  public string? ConcurrencyStamp { get; set; }

  public string? PhoneNumber { get; set; }

  public bool PhoneNumberConfirmed { get; set; }

  public bool TwoFactorEnabled { get; set; }

  public DateTime? LockoutEnd { get; set; }

  public bool LockoutEnabled { get; set; }

  public int AccessFailedCount { get; set; }

  public string? RememberToken { get; set; }

  public DateTime? CreatedAt { get; set; }

  public DateTime? UpdatedAt { get; set; }
  public virtual ICollection<Article> Articles
  { get; set; } = new List<Article>();
  public virtual ICollection<Appointment> Appointments
  { get; set; } = new List<Appointment>();
  public virtual ICollection<Order> Orders
  { get; set; } = new List<Order>();
  public virtual ICollection<Feedback> Feedbacks
  { get; set; } = new List<Feedback>();
  public virtual ICollection<TherapistOpenTime> TherapistOpenTimes
  { get; set; } = new List<TherapistOpenTime>();
  public virtual ICollection<TherapistOpenService> TherapistOpenServices
  { get; set; } = new List<TherapistOpenService>();
}
