namespace SpendWise.Application.Accounts;

public interface IAccountService
{
    Task<IReadOnlyList<AccountDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AccountDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AccountDto> CreateAsync(CreateAccountRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateAccountRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
