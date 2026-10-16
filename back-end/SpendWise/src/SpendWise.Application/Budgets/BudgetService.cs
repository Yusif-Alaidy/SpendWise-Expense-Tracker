using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FluentValidation;
using FluentValidation.Results;
using SpendWise.Application.Budgets.Dtos;
using SpendWise.Application.Common.Interfaces;
using SpendWise.Domain.Entities;

namespace SpendWise.Application.Budgets;

public class BudgetService : IBudgetService
{
    private readonly IAppDbContext _db;
    private readonly IBudgetPeriodResolver _periodResolver;
    private readonly IValidator<UpsertBudgetRequest> _upsertValidator;
    private readonly IValidator<BudgetQueryParams> _queryValidator;
    private readonly ILogger<BudgetService> _logger;

    public BudgetService(IAppDbContext db, IBudgetPeriodResolver periodResolver, IValidator<UpsertBudgetRequest> upsertValidator, IValidator<BudgetQueryParams> queryValidator, ILogger<BudgetService> logger)
    {
        _db = db;
        _periodResolver = periodResolver;
        _upsertValidator = upsertValidator;
        _queryValidator = queryValidator;
        _logger = logger;
    }

    public async Task<BudgetOverviewDto> GetOverviewAsync(BudgetQueryParams query, CancellationToken cancellationToken = default)
    {
        await _queryValidator.ValidateAndThrowAsync(query, cancellationToken);

        var (year, month) = _periodResolver.Resolve(query.Year, query.Month);
        var (start, end) = _periodResolver.GetRange(year, month);

        // Single query: for each budget get category name/color and spent as sum of expense transactions in range
        var items = await _db.Budgets.AsNoTracking()
            .Where(b => b.Year == year && b.Month == month)
            .Select(b => new
            {
                b.Id,
                b.CategoryId,
                CategoryName = b.Category.Name,
                CategoryColor = b.Category.Color,
                b.LimitAmount,
                Spent = _db.Transactions.Where(t => t.CategoryId == b.CategoryId && t.Type == Domain.Enums.TransactionType.Expense && t.Date >= start && t.Date < end).Sum(t => (decimal?)t.Amount) ?? 0m,
                b.Year,
                b.Month
            })
            .ToListAsync(cancellationToken);

        var statusItems = items.OrderBy(i => i.CategoryName).Select(i =>
        {
            var remaining = i.LimitAmount - i.Spent;
            var percent = i.LimitAmount == 0 ? 0m : Math.Round(i.Spent / i.LimitAmount * 100m, 1, MidpointRounding.AwayFromZero);
            var exceeded = i.Spent > i.LimitAmount;
            return new BudgetStatusDto(i.Id, i.CategoryId, i.CategoryName ?? string.Empty, i.CategoryColor ?? string.Empty, i.Year, i.Month, i.LimitAmount, i.Spent, remaining, percent, exceeded);
        }).ToList();

        var totalLimit = statusItems.Sum(x => x.LimitAmount);
        var totalSpent = statusItems.Sum(x => x.Spent);

        return new BudgetOverviewDto(year, month, totalLimit, totalSpent, statusItems);
    }

    public async Task<BudgetStatusDto> UpsertAsync(UpsertBudgetRequest request, CancellationToken cancellationToken = default)
    {
        await _upsertValidator.ValidateAndThrowAsync(request, cancellationToken);

        var (year, month) = _periodResolver.Resolve(request.Year, request.Month);

        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);
        if (category is null)
            throw new ValidationException(new[] { new ValidationFailure("CategoryId", "Category does not exist.") });

        if (category.Type != Domain.Enums.TransactionType.Expense)
            throw new ValidationException(new[] { new ValidationFailure("CategoryId", "Budgets can only be set for expense categories.") });

        var existing = await _db.Budgets.FirstOrDefaultAsync(b => b.CategoryId == request.CategoryId && b.Year == year && b.Month == month, cancellationToken);
        if (existing is null)
        {
            existing = new Budget { CategoryId = request.CategoryId, LimitAmount = request.LimitAmount, Year = year, Month = month };
            _db.Budgets.Add(existing);
            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                throw new SpendWise.Application.Common.Exceptions.ConflictException("A budget already exists for this category and month.");
            }
        }
        else
        {
            existing.LimitAmount = request.LimitAmount;
            await _db.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Budget upserted {BudgetId}", existing.Id);

        // Compute spent for returned DTO
        var (start, end) = _periodResolver.GetRange(year, month);
        var spent = await _db.Transactions.Where(t => t.CategoryId == existing.CategoryId && t.Type == Domain.Enums.TransactionType.Expense && t.Date >= start && t.Date < end)
            .SumAsync(t => (decimal?)t.Amount, cancellationToken) ?? 0m;

        var remaining = existing.LimitAmount - spent;
        var percent = existing.LimitAmount == 0 ? 0m : Math.Round(spent / existing.LimitAmount * 100m, 1, MidpointRounding.AwayFromZero);
        var exceeded = spent > existing.LimitAmount;

        return new BudgetStatusDto(existing.Id, existing.CategoryId, category.Name ?? string.Empty, category.Color ?? string.Empty, existing.Year, existing.Month, existing.LimitAmount, spent, remaining, percent, exceeded);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _db.Budgets.FindAsync(new object[] { id }, cancellationToken);
        if (existing is null)
            throw new SpendWise.Application.Common.Exceptions.NotFoundException("Budget not found.");

        _db.Budgets.Remove(existing);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Budget deleted {BudgetId}", id);
    }
}
