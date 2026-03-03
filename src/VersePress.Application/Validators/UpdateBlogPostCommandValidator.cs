using FluentValidation;
using VersePress.Application.Commands;

namespace VersePress.Application.Validators;

/// <summary>
/// Validator for UpdateBlogPostCommand ensuring bilingual content meets requirements
/// </summary>
public class UpdateBlogPostCommandValidator : AbstractValidator<UpdateBlogPostCommand>
{
    public UpdateBlogPostCommandValidator()
    {
        // TitleEn validation: 5-200 characters
        RuleFor(x => x.TitleEn)
            .NotEmpty()
            .WithMessage("English title is required")
            .Length(5, 200)
            .WithMessage("English title must be between 5 and 200 characters");

        // TitleAr validation: 5-200 characters
        RuleFor(x => x.TitleAr)
            .NotEmpty()
            .WithMessage("Arabic title is required")
            .Length(5, 200)
            .WithMessage("Arabic title must be between 5 and 200 characters");

        // ContentEn validation: minimum 100 characters
        RuleFor(x => x.ContentEn)
            .NotEmpty()
            .WithMessage("English content is required")
            .MinimumLength(100)
            .WithMessage("English content must be at least 100 characters");

        // ContentAr validation: minimum 100 characters
        RuleFor(x => x.ContentAr)
            .NotEmpty()
            .WithMessage("Arabic content is required")
            .MinimumLength(100)
            .WithMessage("Arabic content must be at least 100 characters");
    }
}
