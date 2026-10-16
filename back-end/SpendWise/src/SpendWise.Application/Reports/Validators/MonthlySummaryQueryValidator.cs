using FluentValidation;
using SpendWise.Application.Reports.Dtos;

namespace SpendWise.Application.Reports.Validators;

public class MonthlySummaryQueryValidator : AbstractValidator<MonthlySummaryQueryParams>
{
    public MonthlySummaryQueryValidator()
    {
        When(x => x.Year.HasValue, () => RuleFor(x => x.Year).InclusiveBetween(2000, 2100));
        When(x => x.Month.HasValue, () => RuleFor(x => x.Month).InclusiveBetween(1, 12));
        // Do not duplicate resolver's cross-field validation here; keep basic ranges only.
    }
}
