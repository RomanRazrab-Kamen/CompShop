using System;
using System.Collections.Generic;

namespace CompShop.Models;

public partial class RepairStatus
{
    public int Id { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Repair> Repairs { get; set; } = new List<Repair>();
}
