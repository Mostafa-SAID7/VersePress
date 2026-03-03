using FluentValidation;
using VersePress.Application.Commands;

namespace VersePress.Application.Validators;

/// <summary>
/// Validator for CreateProjectCommand ensuring bilingual names are provided
/// </summary>
public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        // NameEn validation: required and not empty
        RuleFor(x => x.NameEn)
            .NotEmpty()
            .WithMessage("English name is required")
            .MaximumLength(100)
            .WithMessage("English name must not exceed 100 characters");

        // NameAr validation: required and not empty
        RuleFor(x => x.NameAr)
            .NotEmpty()
            .WithMessage("Arabic name is required")
            .MaximumLength(100)
            .WithMessage("Arabic name must not exceed 100 characters");
    }
}
