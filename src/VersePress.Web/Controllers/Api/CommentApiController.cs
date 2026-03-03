using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VersePress.Application.Commands;
using VersePress.Application.Interfaces;

namespace VersePress.Web.Controllers.Api;

[ApiController]
[Route("api/comments")]
[Authorize]
public class CommentApiController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly ILogger<CommentApiController> _logger;

    public CommentApiController(
        ICommentService commentService,
        ILogger<CommentApiController> logger)
    {
        _commentService = commentService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentCommand command)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            command.UserId = userId;

            var result = await _commentService.CreateCommentAsync(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating comment");
            return StatusCode(500, new { error = "An error occurred while creating the comment" });
        }
    }
}
