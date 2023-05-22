using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace HomeTherapistApi.Models;

public partial class Appointment
{
  public ulong Id { get; set; }

  public string UserId { get; set; } = null!;
  public DateTime? StartDt { get; set; }

  public string CustomerId { get; set; } = null!;

  public string CustomerPhone { get; set; } = null!;

  public string CustomerAddress { get; set; } = null!;

  public Point CustomerLocation { get; set; } = null!;

  public bool IsComplete { get; set; }

  public DateTime? CreatedAt { get; set; }

  public DateTime? UpdatedAt { get; set; }
  public virtual User User { get; set; } = null!;
  public virtual Calendar Calendar { get; set; } = null!;

  public virtual ICollection<AppointmentDetail> AppointmentDetails
  { get; set; } = new List<AppointmentDetail>();
}
