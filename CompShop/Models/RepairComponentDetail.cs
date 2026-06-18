using System;
using System.Collections.Generic;

namespace CompShop.Models;

public partial class RepairComponentDetail
{
    public int Id { get; set; }

    public int RepairId { get; set; }

    public int ComponentId { get; set; }

    public int Quantity { get; set; }

    public decimal FixedPrice { get; set; }

    public virtual Component Component { get; set; } = null!;

    public virtual Repair Repair { get; set; } = null!;
}
