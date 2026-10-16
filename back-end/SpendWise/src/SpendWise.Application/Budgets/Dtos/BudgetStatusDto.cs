namespace SpendWise.Application.Budgets.Dtos;

public record BudgetStatusDto(int Id, int CategoryId, string CategoryName, string CategoryColor, int Year, int Month, decimal LimitAmount, decimal Spent, decimal Remaining, decimal PercentUsed, bool IsExceeded);
