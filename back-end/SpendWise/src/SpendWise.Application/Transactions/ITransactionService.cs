using SpendWise.Application.Common;

namespace SpendWise.Application.Transactions;

public interface ITransactionService
{
    Task<PagedResult<TransactionDto>> GetPagedAsync(TransactionQueryParams query, CancellationToken cancellationToken = default);
    Task<TransactionDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TransactionDto> CreateAsync(CreateTransactionRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateTransactionRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
