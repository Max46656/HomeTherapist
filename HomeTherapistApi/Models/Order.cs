using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace HomeTherapistApi.Models;

public partial class Order
{
  public ulong Id { get; set; }

  public string UserId { get; set; } = null!;

  public DateTime? StartDt { get; set; }

  public string CustomerId { get; set; } = null!;

  public string CustomerPhone { get; set; } = null!;

  public string CustomerAddress { get; set; } = null!;

  public decimal Latitude { get; set; }

  public decimal Longitude { get; set; }
  public string? Gender { get; set; }

  public string? AgeGroup { get; set; }

  public bool IsComplete { get; set; }

  public DateTime? CreatedAt { get; set; }

  public DateTime? UpdatedAt { get; set; }
  public virtual User User { get; set; } = null!;
  public virtual Calendar Calendar { get; set; } = null!;
  public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
  public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
