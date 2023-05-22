using System;
using System.Collections.Generic;

namespace HomeTherapistApi.Models;

public partial class OrderDetail
{
    public ulong Id { get; set; }

    public ulong OrderId { get; set; }

    public ulong ServiceId { get; set; }

    public double Price { get; set; }

    public string Note { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
