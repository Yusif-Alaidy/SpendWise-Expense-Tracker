// SpendWise.Domain/Entities/Transaction.cs
using SpendWise.Domain.Common;
using SpendWise.Domain.Enums;

namespace SpendWise.Domain.Entities;

public class Transaction : BaseEntity
{
    public int AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public DateOnly Date { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TransactionTag> TransactionTags { get; set; } = new List<TransactionTag>();
}