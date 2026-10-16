using SpendWise.Domain.Enums;

namespace SpendWise.Application.Accounts;

public record CreateAccountRequest(string Name, AccountType Type, decimal OpeningBalance);
