using System;
using System.Collections.Generic;

namespace HomeTherapistApi.Models;

public partial class TherapistOpenTime
{
  public ulong Id { get; set; }

  public string UserId { get; set; } = null!;

  public DateTime? StartDt { get; set; }

  public DateTime? CreatedAt { get; set; }

  public DateTime? UpdatedAt { get; set; }
  public virtual User User { get; set; } = null!;
  public virtual Calendar Calendar { get; set; } = null!;
}
