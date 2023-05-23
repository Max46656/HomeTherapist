using System;
using System.Collections.Generic;

namespace HomeTherapistApi.Models;

public partial class Calendar
{
  public ulong Id { get; set; }

  public DateOnly? Date { get; set; }

  public DateTime? Dt { get; set; }

  public ushort? Year { get; set; }

  public byte? Quarter { get; set; }

  public byte? Month { get; set; }

  public byte? Day { get; set; }

  public byte? DayOfWeek { get; set; }

  public byte? WeekOfYear { get; set; }

  public bool? IsWeekend { get; set; }

  public bool? IsHoliday { get; set; }

  public virtual ICollection<Appointment> Appointments
  { get; set; } = new List<Appointment>();
  public virtual ICollection<Order> Orders
  { get; set; } = new List<Order>();
  public virtual ICollection<TherapistOpenTime> TherapistOpenTimes
  { get; set; } = new List<TherapistOpenTime>();
}