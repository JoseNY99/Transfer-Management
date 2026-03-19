using Microsoft.EntityFrameworkCore;
using Payments.Domain.Entities;

namespace Risk.Worker.Persistence;

public sealed class RiskReadDbContext : DbContext
{
    public DbSet<Payment> Payments => Set<Payment>();

    public RiskReadDbContext(DbContextOptions<RiskReadDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(builder =>
        {
            builder.ToTable("payments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.ExternalOperationId)
                .HasColumnName("external_operation_id");

            builder.Property(x => x.CustomerId)
                .HasColumnName("customer_id");

            builder.Property(x => x.ServiceProviderId)
                .HasColumnName("service_provider_id");

            builder.Property(x => x.PaymentMethodId)
                .HasColumnName("payment_method_id");

            builder.Property(x => x.Amount)
                .HasColumnName("amount");

            builder.Property(x => x.Status)
                .HasColumnName("status")
                .HasConversion<string>();

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at");

            builder.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at");
        });
    }
}