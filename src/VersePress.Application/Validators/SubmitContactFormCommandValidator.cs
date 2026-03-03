using FluentValidation;
using VersePress.Application.Commands;

namespace VersePress.Application.Validators;

/// <summary>
/// Validator for SubmitContactFormCommand with email format validation
/// </summary>
public class SubmitContactFormCommandValidator : AbstractValidator<SubmitContactFormCommand>
{
    public SubmitContactFormCommandValidator()
    {
        // Name validation: 2-100 characters
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .Length(2, 100)
            .WithMessage("Name must be between 2 and 100 characters");

        // Email validation: required and valid format
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email must be a valid email address");

        // Subject validation: 5-200 characters
        RuleFor(x => x.Subject)
            .NotEmpty()
            .WithMessage("Subject is required")
            .Length(5, 200)
            .WithMessage("Subject must be between 5 and 200 characters");

        // Message validation: 10-5000 characters
        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage("Message is required")
            .Length(10, 5000)
            .WithMessage("Message must be between 10 and 5000 characters");
    }
}
