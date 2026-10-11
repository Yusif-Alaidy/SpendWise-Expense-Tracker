// SpendWise.Domain/Entities/Budget.cs
using SpendWise.Domain.Common;

namespace SpendWise.Domain.Entities;

public class Budget : BaseEntity
{
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public decimal LimitAmount { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
}