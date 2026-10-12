// CategoryConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpendWise.Domain.Entities;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.Color).IsRequired().HasMaxLength(7);

        builder.HasIndex(c => new { c.UserId, c.Type, c.Name }).IsUnique();

        builder.HasMany(c => c.Transactions)
               .WithOne(t => t.Category)
               .HasForeignKey(t => t.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Budgets)
               .WithOne(b => b.Category)
               .HasForeignKey(b => b.CategoryId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}