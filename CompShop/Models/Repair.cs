using System;
using System.Collections.Generic;

namespace CompShop.Models;

public partial class Repair
{
    public int Id { get; set; }

    public string DeviceName { get; set; } = null!;

    public decimal? TotalCost { get; set; }

    public DateTime DateReceived { get; set; }

    public string? ProblemDescription { get; set; }

    public int? WarrantyPeriod { get; set; }

    public int StatusId { get; set; }

    public int MasterId { get; set; }

    public int ManagerId { get; set; }

    public virtual Employee Manager { get; set; } = null!;

    public virtual Employee Master { get; set; } = null!;

    public virtual ICollection<RepairComponentDetail> RepairComponentDetails { get; set; } = new List<RepairComponentDetail>();

    public virtual RepairStatus Status { get; set; } = null!;
}
