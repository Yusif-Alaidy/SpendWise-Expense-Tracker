using SpendWise.Application.Budgets.Dtos;

namespace SpendWise.Application.Budgets;

public interface IBudgetService
{
    Task<BudgetOverviewDto> GetOverviewAsync(Dtos.BudgetQueryParams query, CancellationToken cancellationToken = default);
    Task<BudgetStatusDto> UpsertAsync(Dtos.UpsertBudgetRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
