using FluentValidation;
using SpendWise.Application.Budgets.Dtos;

namespace SpendWise.Application.Budgets.Validators;

public class BudgetQueryValidator : AbstractValidator<BudgetQueryParams>
{
    public BudgetQueryValidator()
    {
        When(x => x.Year.HasValue, () => RuleFor(x => x.Year).InclusiveBetween(2000, 2100));
        When(x => x.Month.HasValue, () => RuleFor(x => x.Month).InclusiveBetween(1, 12));
    }
}
