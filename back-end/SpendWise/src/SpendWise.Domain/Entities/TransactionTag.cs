// SpendWise.Domain/Entities/TransactionTag.cs
namespace SpendWise.Domain.Entities;

// Explicit join entity for the many-to-many (composite key configured in Infrastructure)
public class TransactionTag
{
    public int TransactionId { get; set; }
    public Transaction Transaction { get; set; } = null!;

    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}