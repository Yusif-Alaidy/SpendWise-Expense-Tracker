using SpendWise.Domain.Entities;
using SpendWise.Domain.Enums;
using System;

namespace SpendWise.Tests.Infrastructure;

public static class TestData
{
    public static Guid NewUserId() => Guid.NewGuid();

    public static Account CreateAccount(Guid userId, string name = "Cash", decimal opening = 0m)
        => new Account { UserId = userId, Name = name, Type = AccountType.Cash, OpeningBalance = opening };

    public static Category CreateCategory(Guid userId, string name, TransactionType type)
        => new Category { UserId = userId, Name = name, Type = type, Color = "#000000" };

    public static Transaction CreateTransaction(int accountId, int categoryId, decimal amount, TransactionType type, DateOnly date, string? note = null)
        => new Transaction { AccountId = accountId, CategoryId = categoryId, Amount = amount, Type = type, Date = date, Note = note, CreatedAt = DateTime.UtcNow };

    public static Tag CreateTag(Guid userId, string name) => new Tag { UserId = userId, Name = name };

    public static Budget CreateBudget(int categoryId, int year, int month, decimal limit)
        => new Budget { CategoryId = categoryId, Year = year, Month = month, LimitAmount = limit };
}
