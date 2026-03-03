using Microsoft.AspNetCore.Mvc;
using VersePress.Application.Interfaces;
using VersePress.Domain.Enums;

namespace VersePress.Web.Controllers.Api;

[ApiController]
[Route("api/shares")]
public class ShareApiController : ControllerBase
{
    private readonly IShareTrackingService _shareTrackingService;
    private readonly ILogger<ShareApiController> _logger;

    public ShareApiController(
        IShareTrackingService shareTrackingService,
        ILogger<ShareApiController> logger)
    {
        _shareTrackingService = shareTrackingService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> RecordShare([FromBody] RecordShareRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _shareTrackingService.RecordShareAsync(request.BlogPostId, request.Platform);
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording share");
            return StatusCode(500, new { error = "An error occurred while recording the share" });
        }
    }

    public class RecordShareRequest
    {
        public Guid BlogPostId { get; set; }
        public Platform Platform { get; set; }
    }
}
