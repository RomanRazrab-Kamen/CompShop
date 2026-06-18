using System;
using System.Collections.Generic;

namespace CompShop.Models;

public partial class StockTransaction
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public string TransactionType { get; set; } = null!;

    public DateTime TransactionDate { get; set; }

    public int WarehouseId { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<TransactionDetail> TransactionDetails { get; set; } = new List<TransactionDetail>();

    public virtual Warehouse Warehouse { get; set; } = null!;
}
