using System;
using System.Collections.Generic;

namespace CompShop.Models;

public partial class Category
{
    public int Id { get; set; }

    public string CategoryName { get; set; } = null!;

    public virtual ICollection<Component> Components { get; set; } = new List<Component>();
}
