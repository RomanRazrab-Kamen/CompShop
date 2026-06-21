using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CompShop.Models;

public partial class CompShopDbContext : DbContext
{
    public CompShopDbContext()
    {
    }

    public CompShopDbContext(DbContextOptions<CompShopDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Component> Components { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Repair> Repairs { get; set; }

    public virtual DbSet<RepairComponentDetail> RepairComponentDetails { get; set; }

    public virtual DbSet<RepairStatus> RepairStatuses { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<StockBalance> StockBalances { get; set; }

    public virtual DbSet<StockTransaction> StockTransactions { get; set; }

    public virtual DbSet<TransactionDetail> TransactionDetails { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=ROOM\\SQLEXPRESS;Database=CompShopDB;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3214EC27866F8845");

            entity.ToTable("Category");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Component>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Componen__3214EC277D6DC11B");

            entity.ToTable("Component");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.ComponentName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Category).WithMany(p => p.Components)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Component_Category");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC27F76ADA5E");

            entity.ToTable("Employee");

            entity.HasIndex(e => e.Login, "UQ__Employee__5E55825B9D9F50F2").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Login)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MiddleName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(11)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");

            entity.HasOne(d => d.Role).WithMany(p => p.Employees)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Role");
        });

        modelBuilder.Entity<Repair>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Repair__3214EC27C1014086");

            entity.ToTable("Repair");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DateReceived)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeviceName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ManagerId).HasColumnName("ManagerID");
            entity.Property(e => e.MasterId).HasColumnName("MasterID");
            entity.Property(e => e.ProblemDescription).HasColumnType("text");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.TotalCost)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.WarrantyPeriod).HasDefaultValue(0);

            entity.HasOne(d => d.Manager).WithMany(p => p.RepairManagers)
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Repair_Manager");

            entity.HasOne(d => d.Master).WithMany(p => p.RepairMasters)
                .HasForeignKey(d => d.MasterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Repair_Master");

            entity.HasOne(d => d.Status).WithMany(p => p.Repairs)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Repair_Status");
        });

        modelBuilder.Entity<RepairComponentDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RepairCo__3214EC276C5A2446");

            entity.ToTable("RepairComponentDetail");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ComponentId).HasColumnName("ComponentID");
            entity.Property(e => e.FixedPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RepairId).HasColumnName("RepairID");

            entity.HasOne(d => d.Component).WithMany(p => p.RepairComponentDetails)
                .HasForeignKey(d => d.ComponentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Detail_Component");

            entity.HasOne(d => d.Repair).WithMany(p => p.RepairComponentDetails)
                .HasForeignKey(d => d.RepairId)
                .HasConstraintName("FK_Detail_Repair");
        });

        modelBuilder.Entity<RepairStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RepairSt__3214EC27B5C82BE7");

            entity.ToTable("RepairStatus");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC27AEE7A9E4");

            entity.ToTable("Role");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<StockBalance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StockBal__3214EC271B977408");

            entity.ToTable("StockBalance");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ComponentId).HasColumnName("ComponentID");
            entity.Property(e => e.WarehouseId).HasColumnName("WarehouseID");

            entity.HasOne(d => d.Component).WithMany(p => p.StockBalances)
                .HasForeignKey(d => d.ComponentId)
                .HasConstraintName("FK_StockBalance_Component");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.StockBalances)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockBalance_Warehouse");
        });

        modelBuilder.Entity<StockTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StockTra__3214EC27578CA42D");

            entity.ToTable("StockTransaction");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.TransactionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TransactionType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Arrival");
            entity.Property(e => e.WarehouseId).HasColumnName("WarehouseID");

            entity.HasOne(d => d.Employee).WithMany(p => p.StockTransactions)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_Employee");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.StockTransactions)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("FK_StockTransaction_Warehouse");
        });

        modelBuilder.Entity<TransactionDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transact__3214EC276D1C75F2");

            entity.ToTable("TransactionDetail", tb => tb.HasTrigger("TR_SyncStockBalance"));

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ComponentId).HasColumnName("ComponentID");
            entity.Property(e => e.TransactionId).HasColumnName("TransactionID");

            entity.HasOne(d => d.Component).WithMany(p => p.TransactionDetails)
                .HasForeignKey(d => d.ComponentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TransDetail_Component");

            entity.HasOne(d => d.Transaction).WithMany(p => p.TransactionDetails)
                .HasForeignKey(d => d.TransactionId)
                .HasConstraintName("FK_TransDetail_Transaction");
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Warehous__3214EC27AC4BE1FD");

            entity.ToTable("Warehouse");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.WarehouseName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
