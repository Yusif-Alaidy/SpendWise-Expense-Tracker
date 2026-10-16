using SpendWise.Domain.Enums;

namespace SpendWise.Application.Transactions;

public record UpdateTransactionRequest(int AccountId, int CategoryId, decimal Amount, TransactionType Type, DateOnly Date, string? Note, IReadOnlyList<int>? TagIds = null);
