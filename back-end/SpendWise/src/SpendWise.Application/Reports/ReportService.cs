using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FluentValidation;
using SpendWise.Application.Reports.Dtos;
using SpendWise.Application.Common.Interfaces;
using SpendWise.Domain.Enums;
using SpendWise.Application.Budgets;

namespace SpendWise.Application.Reports;

public class ReportService : IReportService
{
    private readonly IAppDbContext _db;
    private readonly IBudgetPeriodResolver _periodResolver;
    private readonly IValidator<MonthlySummaryQueryParams> _monthlyValidator;
    private readonly IValidator<CategoryBreakdownQueryParams> _breakdownValidator;
    private readonly ILogger<ReportService> _logger;

    public ReportService(IAppDbContext db, IBudgetPeriodResolver periodResolver, IValidator<MonthlySummaryQueryParams> monthlyValidator, IValidator<CategoryBreakdownQueryParams> breakdownValidator, ILogger<ReportService> logger)
    {
        _db = db;
        _periodResolver = periodResolver;
        _monthlyValidator = monthlyValidator;
        _breakdownValidator = breakdownValidator;
        _logger = logger;
    }

    public async Task<MonthlySummaryDto> GetMonthlySummaryAsync(MonthlySummaryQueryParams query, CancellationToken cancellationToken = default)
    {
        await _monthlyValidator.ValidateAndThrowAsync(query, cancellationToken);
        var (year, month) = _periodResolver.Resolve(query.Year, query.Month);
        var (start, end) = _periodResolver.GetRange(year, month);

        _logger.LogInformation("Generating monthly summary for {Year}-{Month}", year, month);

        var rows = await _db.Transactions.AsNoTracking()
            .Where(t => t.Date >= start && t.Date < end)
            .GroupBy(t => t.Type)
            .Select(g => new { Type = g.Key, Total = g.Sum(x => (decimal?)x.Amount) ?? 0m, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var incomeRow = rows.FirstOrDefault(r => r.Type == TransactionType.Income);
        var expenseRow = rows.FirstOrDefault(r => r.Type == TransactionType.Expense);

        var totalIncome = incomeRow?.Total ?? 0m;
        var totalExpense = expenseRow?.Total ?? 0m;
        var net = totalIncome - totalExpense;
        decimal? savingsRate = null;
        if (totalIncome != 0m)
            savingsRate = Math.Round(net / totalIncome * 100m, 1, MidpointRounding.AwayFromZero);

        var txCount = (incomeRow?.Count ?? 0) + (expenseRow?.Count ?? 0);

        return new MonthlySummaryDto(year, month, totalIncome, totalExpense, net, savingsRate, txCount);
    }

    public async Task<CategoryBreakdownDto> GetCategoryBreakdownAsync(CategoryBreakdownQueryParams query, CancellationToken cancellationToken = default)
    {
        await _breakdownValidator.ValidateAndThrowAsync(query, cancellationToken);

        DateOnly from, toExclusive;
        TransactionType type;
        if (!query.From.HasValue || !query.To.HasValue)
        {
            var (y, m) = _periodResolver.Resolve(null, null);
            var range = _periodResolver.GetRange(y, m);
            from = range.Start;
            toExclusive = range.EndExclusive;
        }
        else
        {
            from = query.From.Value;
            // inclusive to is provided; convert to exclusive end
            toExclusive = query.To.Value.AddDays(1);
        }

        type = query.Type ?? TransactionType.Expense;

        _logger.LogInformation("Generating category breakdown for {From} to {To} type {Type}", from, toExclusive.AddDays(-1), type);

        var rows = await _db.Transactions.AsNoTracking()
            .Where(t => t.Type == type && t.Date >= from && t.Date < toExclusive)
            .GroupBy(t => new { t.CategoryId, t.Category.Name, t.Category.Color })
            .Select(g => new { g.Key.CategoryId, CategoryName = g.Key.Name, CategoryColor = g.Key.Color, Total = g.Sum(x => (decimal?)x.Amount) ?? 0m, Count = g.Count() })
            .OrderByDescending(x => x.Total).ThenBy(x => x.CategoryName)
            .ToListAsync(cancellationToken);

        var grandTotal = rows.Sum(r => r.Total);
        var items = rows.Select(r => new CategoryBreakdownItemDto(r.CategoryId, r.CategoryName ?? string.Empty, r.CategoryColor ?? string.Empty, r.Total, grandTotal == 0m ? 0m : Math.Round(r.Total / grandTotal * 100m, 1, MidpointRounding.AwayFromZero), r.Count)).ToList();

        return new CategoryBreakdownDto(from, toExclusive.AddDays(-1), type, grandTotal, items);
    }
}
