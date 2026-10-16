using SpendWise.Application.Reports.Dtos;

namespace SpendWise.Application.Reports;

public interface IReportService
{
    Task<MonthlySummaryDto> GetMonthlySummaryAsync(Dtos.MonthlySummaryQueryParams query, CancellationToken cancellationToken = default);
    Task<Dtos.CategoryBreakdownDto> GetCategoryBreakdownAsync(Dtos.CategoryBreakdownQueryParams query, CancellationToken cancellationToken = default);
}
