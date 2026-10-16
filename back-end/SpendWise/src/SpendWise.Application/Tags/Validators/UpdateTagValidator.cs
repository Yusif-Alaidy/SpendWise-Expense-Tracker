using FluentValidation;

namespace SpendWise.Application.Tags.Validators;

public class UpdateTagValidator : AbstractValidator<UpdateTagRequest>
{
    public UpdateTagValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50)
            .Must(n => n.Trim().Length > 0).WithMessage("Name must not be empty or whitespace.");
    }
}
