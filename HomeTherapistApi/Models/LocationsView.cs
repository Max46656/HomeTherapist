using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace HomeTherapistApi.Models;

public partial class LocationsView
{
    public string Type { get; set; } = null!;

    public ulong Id { get; set; }

    public Geometry? Location { get; set; }
}
