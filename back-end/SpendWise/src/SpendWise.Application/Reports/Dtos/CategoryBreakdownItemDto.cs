namespace SpendWise.Application.Reports.Dtos;

public record CategoryBreakdownItemDto(int CategoryId, string CategoryName, string CategoryColor, decimal Total, decimal PercentOfTotal, int TransactionCount);
