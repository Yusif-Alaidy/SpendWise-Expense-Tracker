using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SpendWise.Application.Common.Interfaces;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Categories.Validators;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50)
            .Must(n => n.Trim().Length > 0).WithMessage("Name must not be empty or whitespace.");

        RuleFor(x => x.Type).IsInEnum();

        RuleFor(x => x.Color)
            .NotEmpty()
            .Matches("^#[0-9A-Fa-f]{6}$").WithMessage("Color must be a hex value like #FF5733.");
    }
}

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50)
            .Must(n => n.Trim().Length > 0).WithMessage("Name must not be empty or whitespace.");

        RuleFor(x => x.Color)
            .NotEmpty()
            .Matches("^#[0-9A-Fa-f]{6}$").WithMessage("Color must be a hex value like #FF5733.");
    }
}
