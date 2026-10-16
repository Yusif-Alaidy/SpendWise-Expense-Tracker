// SpendWise.Infrastructure/Persistence/AppDbContext.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpendWise.Application.Common.Interfaces;
using SpendWise.Domain.Entities;
using SpendWise.Infrastructure.Identity;

namespace SpendWise.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    private readonly ICurrentUserService _currentUser;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserService currentUser)
        : base(options)
    {
        _currentUser = currentUser;
    }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<TransactionTag> TransactionTags => Set<TransactionTag>();
    public DbSet<Budget> Budgets => Set<Budget>();

    // Read at query time, so each request's context uses its own user
    private Guid? CurrentUserId => _currentUser.UserId;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Fail closed: if there is no user, UserId == null matches nothing
        builder.Entity<Account>().HasQueryFilter(e => e.UserId == CurrentUserId);
        builder.Entity<Category>().HasQueryFilter(e => e.UserId == CurrentUserId);
        builder.Entity<Transaction>().HasQueryFilter(e => e.Account.UserId == CurrentUserId);
        builder.Entity<Tag>().HasQueryFilter(e => e.UserId == CurrentUserId);

        // No UserId of their own, so they follow their owner
        builder.Entity<Budget>().HasQueryFilter(e => e.Category.UserId == CurrentUserId);
        builder.Entity<TransactionTag>().HasQueryFilter(e => e.Transaction.Account.UserId == CurrentUserId);
    }
}