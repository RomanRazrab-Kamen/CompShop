using System;
using System.Collections.Generic;

namespace CompShop.Models;

public partial class Component
{
    public int Id { get; set; }

    public string ComponentName { get; set; } = null!;

    public decimal Price { get; set; }

    public int CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<RepairComponentDetail> RepairComponentDetails { get; set; } = new List<RepairComponentDetail>();

    public virtual ICollection<StockBalance> StockBalances { get; set; } = new List<StockBalance>();

    public virtual ICollection<TransactionDetail> TransactionDetails { get; set; } = new List<TransactionDetail>();
}
