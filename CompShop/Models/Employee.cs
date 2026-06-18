using System;
using System.Collections.Generic;

namespace CompShop.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual ICollection<Repair> RepairManagers { get; set; } = new List<Repair>();

    public virtual ICollection<Repair> RepairMasters { get; set; } = new List<Repair>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();
}
