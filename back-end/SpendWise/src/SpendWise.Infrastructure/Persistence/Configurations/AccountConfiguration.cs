// AccountConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpendWise.Domain.Entities;
using SpendWise.Infrastructure.Identity;

namespace SpendWise.Infrastructure.Persistence.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(a => a.OpeningBalance).HasPrecision(18, 2);

        builder.HasIndex(a => new { a.UserId, a.Name }).IsUnique();

        builder.HasMany(a => a.Transactions)
               .WithOne(t => t.Account)
               .HasForeignKey(t => t.AccountId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
        .WithMany()
        .HasForeignKey(e => e.UserId)
        .OnDelete(DeleteBehavior.Restrict);
    }
}