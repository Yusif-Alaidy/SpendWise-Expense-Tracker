using SpendWise.Domain.Enums;

namespace SpendWise.Application.Accounts;

public record AccountDto(int Id, string Name, AccountType Type, decimal OpeningBalance, decimal CurrentBalance);
