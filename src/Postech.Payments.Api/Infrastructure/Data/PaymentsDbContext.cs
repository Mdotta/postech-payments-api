using Microsoft.EntityFrameworkCore;
using Postech.Payments.Api.Domain.Entities;

namespace Postech.Payments.Api.Infrastructure.Data;

public class PaymentsDbContext:DbContext
{
    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options):base(options)
    {
        
    }
    
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("payment");
            
            entity.HasKey(p => p.Id);
            
            entity.Property(p => p.OrderId).IsRequired();
            entity.Property(p => p.UserId).IsRequired();
            entity.Property(p => p.GameId).IsRequired();
            entity.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(p => p.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
            entity.Property(p => p.CreatedAt).IsRequired();
            
            entity.Property(p => p.ProcessedAt);
            entity.Property(p => p.PaymentMethod).HasMaxLength(50);
            entity.Property(p => p.Message).HasMaxLength(500);

            entity.HasIndex(e => e.OrderId).IsUnique();
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.GameId);
        });
        //TODO: Implementar configuração de entidade Payments        
    }
}