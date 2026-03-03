using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VersePress.Application.Commands;
using VersePress.Application.Interfaces;

namespace VersePress.Web.Controllers.Api;

[ApiController]
[Route("api/reactions")]
[Authorize]
public class ReactionApiController : ControllerBase
{
    private readonly IReactionService _reactionService;
    private readonly ILogger<ReactionApiController> _logger;

    public ReactionApiController(
        IReactionService reactionService,
        ILogger<ReactionApiController> logger)
    {
        _reactionService = reactionService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> AddReaction([FromBody] AddReactionCommand command)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            command.UserId = userId;

            var result = await _reactionService.AddReactionAsync(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding reaction");
            return StatusCode(500, new { error = "An error occurred while adding the reaction" });
        }
    }

    [HttpDelete("{blogPostId}")]
    public async Task<IActionResult> RemoveReaction(Guid blogPostId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

            var command = new RemoveReactionCommand
            {
                BlogPostId = blogPostId,
                UserId = userId
            };

            await _reactionService.RemoveReactionAsync(command);
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing reaction");
            return StatusCode(500, new { error = "An error occurred while removing the reaction" });
        }
    }
}
