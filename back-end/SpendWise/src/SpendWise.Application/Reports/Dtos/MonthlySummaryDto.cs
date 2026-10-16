namespace SpendWise.Application.Reports.Dtos;

public record MonthlySummaryDto(int Year, int Month, decimal TotalIncome, decimal TotalExpense, decimal Net, decimal? SavingsRatePercent, int TransactionCount);
