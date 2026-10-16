using FluentValidation;
using SpendWise.Application.Budgets.Dtos;

namespace SpendWise.Application.Budgets.Validators;

public class UpsertBudgetValidator : AbstractValidator<UpsertBudgetRequest>
{
    public UpsertBudgetValidator()
    {
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.LimitAmount).GreaterThan(0).LessThanOrEqualTo(999_999_999.99m)
            .ScalePrecision(2, 18, false).WithMessage("LimitAmount must have at most 2 decimal places.");
        When(x => x.Year.HasValue || x.Month.HasValue, () =>
        {
            RuleFor(x => x.Year).InclusiveBetween(2000, 2100);
            RuleFor(x => x.Month).InclusiveBetween(1, 12);
        });
    }
}
