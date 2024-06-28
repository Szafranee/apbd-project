using Microsoft.EntityFrameworkCore;
using Project_01.Domain.Clients;
using Project_01.Domain.Contracts;
using Project_01.Domain.Discounts;
using Project_01.Domain.Employees;
using Project_01.Domain.Payments;
using Project_01.Domain.Products;
using Project_01.Domain.RefreshTokens;

namespace Project_01.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public DbSet<Client> Clients { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Discount?> Discounts { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>()
            .HasDiscriminator<ClientType>("ClientType")
            .HasValue<IndividualClient>(ClientType.Individual)
            .HasValue<CorporateClient>(ClientType.Corporate);

        modelBuilder.Entity<IndividualClient>()
            .Property(c => c.PESEL)
            .HasMaxLength(11);

        modelBuilder.Entity<CorporateClient>()
            .Property(c => c.KRS)
            .HasMaxLength(10);

        modelBuilder.Entity<Contract>()
            .HasOne(c => c.Client)
            .WithMany()
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Contract>()
            .HasOne(c => c.Product)
            .WithMany()
            .HasForeignKey(c => c.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Contract)
            .WithMany(c => c.Payments)
            .HasForeignKey(p => p.ContractId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Contract>()
            .Property(c => c.TotalPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Discount>()
            .Property(d => d.PercentageValue)
            .HasPrecision(5, 2);

        modelBuilder.Entity<Discount>()
            .HasOne(d => d.Product)
            .WithMany(p => p.Discounts)
            .HasForeignKey(d => d.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
            .Property(p => p.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Product>()
            .Property(p => p.YearlyLicenseCost)
            .HasPrecision(18, 2);

        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.Employee)
            .WithMany()
            .HasForeignKey(rt => rt.EmployeeId);
    }
}