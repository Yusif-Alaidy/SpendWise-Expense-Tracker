using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SpendWise.Application.Common.Interfaces;
using SpendWise.Domain.Entities;
using SpendWise.Domain.Enums;
using FluentValidation;

namespace SpendWise.Application.Accounts;

public class AccountService : IAccountService
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<CreateAccountRequest> _createValidator;
    private readonly IValidator<UpdateAccountRequest> _updateValidator;
    private readonly ILogger<AccountService> _logger;

    public AccountService(
        IAppDbContext db,
        ICurrentUserService currentUser,
        IValidator<CreateAccountRequest> createValidator,
        IValidator<UpdateAccountRequest> updateValidator,
        ILogger<AccountService> logger)
    {
        _db = db;
        _currentUser = currentUser;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    public async Task<IReadOnlyList<AccountDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var query = _db.Accounts
            .AsNoTracking()
            .OrderBy(a => a.Name)
            .Select(a => new AccountDto(
                a.Id,
                a.Name,
                a.Type,
                a.OpeningBalance,
                a.OpeningBalance
                    + (_db.Transactions.Where(t => t.AccountId == a.Id && t.Type == TransactionType.Income).Sum(t => (decimal?)t.Amount) ?? 0)
                    - (_db.Transactions.Where(t => t.AccountId == a.Id && t.Type == TransactionType.Expense).Sum(t => (decimal?)t.Amount) ?? 0)
            ));

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<AccountDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var a = await _db.Accounts
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(a => new AccountDto(
                a.Id,
                a.Name,
                a.Type,
                a.OpeningBalance,
                a.OpeningBalance
                    + (_db.Transactions.Where(t => t.AccountId == a.Id && t.Type == TransactionType.Income).Sum(t => (decimal?)t.Amount) ?? 0)
                    - (_db.Transactions.Where(t => t.AccountId == a.Id && t.Type == TransactionType.Expense).Sum(t => (decimal?)t.Amount) ?? 0)
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (a is null)
            throw new SpendWise.Application.Common.Exceptions.NotFoundException("Account not found.");

        return a;
    }

    public async Task<AccountDto> CreateAsync(CreateAccountRequest request, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var userId = _currentUser.UserId ?? throw new SpendWise.Application.Common.Exceptions.UnauthorizedException("User is not authenticated.");

        var name = request.Name.Trim();

        var exists = await _db.Accounts.AnyAsync(a => a.Name.ToLower() == name.ToLower(), cancellationToken);
        if (exists)
            throw new SpendWise.Application.Common.Exceptions.ConflictException("An account with this name already exists.");

        var entity = new Account
        {
            UserId = userId,
            Name = name,
            Type = request.Type,
            OpeningBalance = request.OpeningBalance
        };

        _db.Accounts.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Account created {AccountId}", entity.Id);

        return new AccountDto(entity.Id, entity.Name, entity.Type, entity.OpeningBalance, entity.OpeningBalance);
    }

    public async Task UpdateAsync(int id, UpdateAccountRequest request, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var name = request.Name.Trim();

        var entity = await _db.Accounts.FindAsync(new object[] { id }, cancellationToken);
        if (entity is null)
            throw new SpendWise.Application.Common.Exceptions.NotFoundException("Account not found.");

        var exists = await _db.Accounts.AnyAsync(a => a.Id != id && a.Name.ToLower() == name.ToLower(), cancellationToken);
        if (exists)
            throw new SpendWise.Application.Common.Exceptions.ConflictException("An account with this name already exists.");

        entity.Name = name;
        entity.Type = request.Type;
        entity.OpeningBalance = request.OpeningBalance;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Account updated {AccountId}", entity.Id);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Accounts.FindAsync(new object[] { id }, cancellationToken);
        if (entity is null)
            throw new SpendWise.Application.Common.Exceptions.NotFoundException("Account not found.");

        var hasTransactions = await _db.Transactions.AnyAsync(t => t.AccountId == id, cancellationToken);
        if (hasTransactions)
            throw new SpendWise.Application.Common.Exceptions.ConflictException("Account has transactions and cannot be deleted.");

        _db.Accounts.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Account deleted {AccountId}", entity.Id);
    }
}
