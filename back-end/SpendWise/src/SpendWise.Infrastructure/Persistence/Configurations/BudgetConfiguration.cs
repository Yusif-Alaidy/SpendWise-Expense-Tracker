// BudgetConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpendWise.Domain.Entities;

public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> builder)
    {
        builder.Property(b => b.LimitAmount).HasPrecision(18, 2);
        builder.HasIndex(b => new { b.CategoryId, b.Year, b.Month }).IsUnique();
    }
}