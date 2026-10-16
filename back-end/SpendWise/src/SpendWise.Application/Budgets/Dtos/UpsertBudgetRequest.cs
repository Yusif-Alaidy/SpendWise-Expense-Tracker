namespace SpendWise.Application.Budgets.Dtos;

public record UpsertBudgetRequest(int CategoryId, decimal LimitAmount, int? Year = null, int? Month = null);
