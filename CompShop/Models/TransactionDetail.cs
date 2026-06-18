using System;
using System.Collections.Generic;

namespace CompShop.Models;

public partial class TransactionDetail
{
    public int Id { get; set; }

    public int ComponentId { get; set; }

    public int TransactionId { get; set; }

    public int Quantity { get; set; }

    public virtual Component Component { get; set; } = null!;

    public virtual StockTransaction Transaction { get; set; } = null!;
}
