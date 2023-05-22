using System;
using System.Collections.Generic;

namespace HomeTherapistApi.Models;

public partial class TherapistOpenService
{
  public ulong Id { get; set; }

  public ulong ServiceId { get; set; }

  public string UserId { get; set; } = null!;

  public DateTime? CreatedAt { get; set; }

  public DateTime? UpdatedAt { get; set; }
  public virtual User User { get; set; } = null!;
  public virtual Service Service { get; set; } = null!;
}
