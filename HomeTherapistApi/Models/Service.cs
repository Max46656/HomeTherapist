using System;
using System.Collections.Generic;

namespace HomeTherapistApi.Models;

public partial class Service
{
    public ulong Id { get; set; }

    public string Name { get; set; } = null!;

    public double Price { get; set; }

    public bool? Enabled { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AppointmentDetail> AppointmentDetails { get; set; } = new List<AppointmentDetail>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<TherapistOpenService> TherapistOpenServices { get; set; } = new List<TherapistOpenService>();
}
