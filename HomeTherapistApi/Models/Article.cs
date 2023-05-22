using System;
using System.Collections.Generic;

namespace HomeTherapistApi.Models;

public partial class Article
{
  public ulong Id { get; set; }

  public string UserId { get; set; } = null!;

  public string Title { get; set; } = null!;

  public string? Subtitle { get; set; }

  public string Body { get; set; } = null!;

  public DateTime? CreatedAt { get; set; }

  public DateTime? UpdatedAt { get; set; }
  public virtual User User { get; set; } = null!;
}
