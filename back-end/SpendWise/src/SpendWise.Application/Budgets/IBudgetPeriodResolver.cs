using System;

namespace SpendWise.Application.Budgets;

public interface IBudgetPeriodResolver
{
    (int Year, int Month) Resolve(int? year, int? month);
    (DateOnly Start, DateOnly EndExclusive) GetRange(int year, int month);
}
