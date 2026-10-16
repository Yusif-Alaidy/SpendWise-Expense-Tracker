using SpendWise.Domain.Enums;

namespace SpendWise.Application.Transactions;

public record TransactionDto(
    int Id,
    int AccountId,
    string AccountName,
    int CategoryId,
    string CategoryName,
    string CategoryColor,
    decimal Amount,
    TransactionType Type,
    DateOnly Date,
    string? Note,
    DateTime CreatedAt,
    IReadOnlyList<SpendWise.Application.Tags.TagDto> Tags);
