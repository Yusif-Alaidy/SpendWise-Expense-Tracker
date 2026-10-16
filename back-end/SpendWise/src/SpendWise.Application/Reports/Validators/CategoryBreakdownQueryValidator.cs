using FluentValidation;
using SpendWise.Application.Reports.Dtos;

namespace SpendWise.Application.Reports.Validators;

public class CategoryBreakdownQueryValidator : AbstractValidator<CategoryBreakdownQueryParams>
{
    public CategoryBreakdownQueryValidator()
    {
        RuleFor(x => x).Custom((q, ctx) =>
        {
            var from = q.From;
            var to = q.To;
            if (from.HasValue ^ to.HasValue)
            {
                var missing = from.HasValue ? "To" : "From";
                ctx.AddFailure(missing, "From and to must be provided together.");
                return;
            }

            if (from.HasValue && to.HasValue)
            {
                if (from > to)
                {
                    ctx.AddFailure("From", "From must be on or before To.");
                    return;
                }

                if ((to.Value.ToDateTime(TimeOnly.MinValue) - from.Value.ToDateTime(TimeOnly.MinValue)).TotalDays > 366)
                {
                    ctx.AddFailure("From", "Range must not exceed 366 days.");
                    return;
                }

                var min = new DateOnly(2000, 1, 1);
                if (from < min || to < min)
                {
                    ctx.AddFailure("From", "Dates must be on or after 2000-01-01.");
                }
            }
        });
    }
}
