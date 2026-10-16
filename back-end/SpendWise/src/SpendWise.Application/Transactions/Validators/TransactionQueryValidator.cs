using FluentValidation;

namespace SpendWise.Application.Transactions.Validators;

public class TransactionQueryValidator : AbstractValidator<TransactionQueryParams>
{
    public TransactionQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);

        RuleFor(x => x.SortBy).Must(s => string.IsNullOrEmpty(s) || new[] { "date", "amount", "category" }.Contains(s.ToLower()))
            .WithMessage("SortBy must be one of: date, amount, category.");

        RuleFor(x => x.SortDir).Must(s => string.IsNullOrEmpty(s) || new[] { "asc", "desc" }.Contains(s.ToLower()))
            .WithMessage("SortDir must be 'asc' or 'desc'.");

        RuleFor(x => x.Search).MaximumLength(100);
        RuleFor(x => x.TagIds)
            .Must(list => list == null || (list.Count <= 20 && list.All(id => id > 0)))
            .WithMessage("TagIds must contain at most 20 positive ids.");

        RuleFor(x => x).Custom((p, ctx) =>
        {
            if (p.From.HasValue && p.To.HasValue && p.From > p.To)
                ctx.AddFailure("From", "From must be less than or equal to To.");

            if (p.MinAmount.HasValue && p.MaxAmount.HasValue && p.MinAmount > p.MaxAmount)
                ctx.AddFailure("MinAmount", "MinAmount must be less than or equal to MaxAmount.");

            if (p.MinAmount.HasValue && p.MinAmount < 0)
                ctx.AddFailure("MinAmount", "MinAmount must be >= 0.");
            if (p.MaxAmount.HasValue && p.MaxAmount < 0)
                ctx.AddFailure("MaxAmount", "MaxAmount must be >= 0.");
        });
    }
}
