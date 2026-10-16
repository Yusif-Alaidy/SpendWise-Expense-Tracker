using FluentValidation;
using FluentValidation.Results;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Transactions.Validators;

public class CreateTransactionValidator : AbstractValidator<CreateTransactionRequest>
{
    public CreateTransactionValidator(TimeProvider timeProvider)
    {
        var todayUtc = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

        RuleFor(x => x.Amount)
            .GreaterThan(0m)
            .LessThanOrEqualTo(999999999.99m)
            .ScalePrecision(2, 18).WithMessage("Amount must have at most 2 decimal places and precision 18.");

        RuleFor(x => x.Type).IsInEnum();

        RuleFor(x => x.Date)
            .Must(d => d >= new DateOnly(2000, 1, 1) && d <= todayUtc.AddDays(1))
            .WithMessage("Date must be between 2000-01-01 and tomorrow.");

        RuleFor(x => x.Note).MaximumLength(250);

        RuleFor(x => x.AccountId).GreaterThan(0);
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.TagIds)
            .Must(list => list == null || list.Count <= 10).WithMessage("At most 10 tags are allowed.")
            .Must(list => list == null || list.All(id => id > 0)).WithMessage("TagIds must be positive integers.");
    }
}
