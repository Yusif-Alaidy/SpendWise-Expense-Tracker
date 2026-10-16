using SpendWise.Domain.Enums;

namespace SpendWise.Application.Reports.Dtos;

public record CategoryBreakdownQueryParams(DateOnly? From = null, DateOnly? To = null, TransactionType? Type = null);
