// SpendWise.Domain/Entities/Tag.cs
using SpendWise.Domain.Common;
using SpendWise.Domain.Enums;

namespace SpendWise.Domain.Entities;

public class Tag : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<TransactionTag> TransactionTags { get; set; } = new List<TransactionTag>();
}