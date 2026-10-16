using System.Collections.Generic;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Reports.Dtos;

public record CategoryBreakdownDto(DateOnly From, DateOnly To, TransactionType Type, decimal GrandTotal, IReadOnlyList<CategoryBreakdownItemDto> Items);
