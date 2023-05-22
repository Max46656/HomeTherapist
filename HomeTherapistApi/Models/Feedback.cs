using System;
using System.Collections.Generic;

namespace HomeTherapistApi.Models;

public partial class Feedback
{
  public ulong Id { get; set; }

  public ulong OrderId { get; set; }

  public string UserId { get; set; } = null!;

  public string CustomerId { get; set; } = null!;

  public string? Comments { get; set; }

  public uint Rating { get; set; }

  public DateTime? CreatedAt { get; set; }

  public DateTime? UpdatedAt { get; set; }
  public virtual User User { get; set; } = null!;
  public virtual Order Order { get; set; } = null!;
}
