using FluentValidation;
using VersePress.Application.Commands;
using VersePress.Domain.Interfaces;

namespace VersePress.Application.Validators;

/// <summary>
/// Validator for CreateCommentCommand with entity existence checks
/// </summary>
public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly ICommentRepository _commentRepository;

    public CreateCommentCommandValidator(
        IBlogPostRepository blogPostRepository,
        ICommentRepository commentRepository)
    {
        _blogPostRepository = blogPostRepository;
        _commentRepository = commentRepository;

        // Content validation: 1-2000 characters
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Comment content is required")
            .Length(1, 2000)
            .WithMessage("Comment content must be between 1 and 2000 characters");

        // BlogPostId validation: must exist
        RuleFor(x => x.BlogPostId)
            .NotEmpty()
            .WithMessage("Blog post ID is required")
            .MustAsync(async (blogPostId, cancellation) => 
                await _blogPostRepository.ExistsAsync(blogPostId))
            .WithMessage("The specified blog post does not exist");

        // ParentCommentId validation: must exist if provided
        RuleFor(x => x.ParentCommentId)
            .MustAsync(async (parentCommentId, cancellation) =>
            {
                if (!parentCommentId.HasValue)
                    return true;
                return await _commentRepository.ExistsAsync(parentCommentId.Value);
            })
            .WithMessage("The specified parent comment does not exist")
            .When(x => x.ParentCommentId.HasValue);

        // UserId validation
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}
