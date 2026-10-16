using System.Collections.Generic;

namespace SpendWise.Application.Budgets.Dtos;

public record BudgetOverviewDto(int Year, int Month, decimal TotalLimit, decimal TotalSpent, IReadOnlyList<BudgetStatusDto> Items);
