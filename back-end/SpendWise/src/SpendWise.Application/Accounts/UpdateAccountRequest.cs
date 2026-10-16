using SpendWise.Domain.Enums;

namespace SpendWise.Application.Accounts;

public record UpdateAccountRequest(string Name, AccountType Type, decimal OpeningBalance);
