// TransactionConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpendWise.Domain.Entities;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.Property(t => t.Amount).HasPrecision(18, 2);
        builder.Property(t => t.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.Note).HasMaxLength(250);

        // Use scalar properties on the Transaction entity for indexes.
        // Transaction no longer contains a UserId property; index by AccountId instead.
        // Accessing navigation properties in index expressions is not supported by EF Core.
        builder.HasIndex(t => new { t.AccountId, t.Date });
        builder.HasIndex(t => new { t.AccountId, t.CategoryId });
    }
}