using System;
using System.Collections.Generic;

namespace CompShop.Models;

public partial class StockBalance
{
    public int Id { get; set; }

    public int ComponentId { get; set; }

    public int WarehouseId { get; set; }

    public int Quantity { get; set; }

    public virtual Component Component { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}
