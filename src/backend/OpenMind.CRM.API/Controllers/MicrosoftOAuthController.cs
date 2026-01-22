using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenMind.CRM.Application.Services.Interfaces;
using System.Security.Claims;
using OpenMind.CRM.Application.Dtos;

namespace OpenMind.CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MicrosoftOAuthController(IMicrosoftOAuthIntegrationService microsoftService) : ControllerBase
{
    [HttpGet("authorize")]
    public ActionResult<AuthUrlResponse> GetAuthorizationUrl()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var authUrl = microsoftService.GenerateAuthorizationUrl(userId);
        return Ok(new AuthUrlResponse
        {
            AuthorizationUrl = authUrl,
            State = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(userId.ToString())),
            Provider = "Microsoft"
        });
    }

    [HttpGet("callback")]
    [AllowAnonymous]
    public async Task<IActionResult> HandleCallback([FromQuery] string code, [FromQuery] string state)
    {
        var redirectUrl = await microsoftService.HandleAuthorizationCallbackAsync(code, state);
        return Redirect(redirectUrl);
    }

    [HttpGet("emails")]
    public async Task<ActionResult<List<EmailDto>>> GetEmails([FromQuery] int maxResults = 10)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var emails = await microsoftService.GetEmailsAsync(userId, maxResults);
        return Ok(emails);
    }

    [HttpGet("calendar/events")]
    public async Task<ActionResult<List<CalendarEventDto>>> GetCalendarEvents(
        [FromQuery] DateTime? timeMin = null,
        [FromQuery] DateTime? timeMax = null,
        [FromQuery] int maxResults = 50)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var events = await microsoftService.GetCalendarEventsAsync(userId, timeMin, timeMax, maxResults);
        return Ok(events);
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var hasValidToken = await microsoftService.HasValidTokenAsync(userId);
        return Ok(new { Provider = "Microsoft", IsConnected = hasValidToken });
    }

    [HttpDelete("revoke")]
    public async Task<IActionResult> RevokeAccess()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var success = await microsoftService.RevokeTokenAsync(userId);
        return success ? Ok() : BadRequest("Failed to revoke Microsoft access");
    }
}
