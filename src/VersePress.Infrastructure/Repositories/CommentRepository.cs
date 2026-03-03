using Microsoft.EntityFrameworkCore;
using VersePress.Domain.Entities;
using VersePress.Domain.Interfaces;
using VersePress.Infrastructure.Data;

namespace VersePress.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Comment entities with specialized query methods.
/// </summary>
public class CommentRepository : Repository<Comment>, ICommentRepository
{
    /// <summary>
    /// Initializes a new instance of the CommentRepository class.
    /// </summary>
    /// <param name="context">Application database context</param>
    public CommentRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Comment>> GetCommentsByPostAsync(Guid blogPostId)
    {
        return await _dbSet
            .Include(c => c.User)
            .Include(c => c.Replies)
                .ThenInclude(r => r.User)
            .Where(c => c.BlogPostId == blogPostId && c.ParentCommentId == null)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Comment>> GetPendingCommentsAsync()
    {
        return await _dbSet
            .Include(c => c.User)
            .Include(c => c.BlogPost)
            .Where(c => !c.IsApproved)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<int> GetPendingCommentCountAsync()
    {
        return await _dbSet.CountAsync(c => !c.IsApproved);
    }
}
