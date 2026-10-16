using FluentValidation;

namespace SpendWise.Application.Accounts.Validators;

public class UpdateAccountValidator : AbstractValidator<UpdateAccountRequest>
{
    public UpdateAccountValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .Must(n => n.Trim().Length > 0).WithMessage("Name must not be empty or whitespace.");

        RuleFor(x => x.Type).IsInEnum();

        RuleFor(x => x.OpeningBalance)
            .InclusiveBetween(-999999999.99m, 999999999.99m)
            .ScalePrecision(2, 18).WithMessage("OpeningBalance must have at most 2 decimal places and precision 18.");
    }
}
