// SpendWise.Domain/Entities/Account.cs
using SpendWise.Domain.Common;
using SpendWise.Domain.Enums;

namespace SpendWise.Domain.Entities;

public class Account : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public AccountType Type { get; set; }
    public decimal OpeningBalance { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}