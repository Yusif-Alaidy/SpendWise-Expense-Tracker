// SpendWise.Domain/Entities/Category.cs
using SpendWise.Domain.Common;
using SpendWise.Domain.Enums;
using System.Transactions;

namespace SpendWise.Domain.Entities;

public class Category : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public string Color { get; set; } = "#888888";

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
}