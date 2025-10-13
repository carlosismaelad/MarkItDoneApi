using Microsoft.AspNetCore.Mvc;
using MarkItDoneApi.Src.V1.Session.Services;
using MarkItDoneApi.Src.V1.Session.Rest.DTO;

namespace MarkItDoneApi.Src.V1.Session.Rest;

[ApiController]
[Route("api/v1/sessions")]
public class SessionController : ControllerBase
{
    private readonly SessionService _sessionService;

    public SessionController(SessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSessionAsync([FromBody] SessionRequest request)
    {
        var authenticatedUser = await _sessionService.GetAuthenticatedUserAsync(
            request.Email,
            request.Password
        );

        var newSession = await _sessionService.CreateSessionAsync(authenticatedUser.Id);

        SetSessionCookie(newSession.Token);

        return StatusCode(201, new { message = "Sessão criada com sucesso" });
    }

    private void SetSessionCookie(string sessionToken)
    {
        var cookieOptions = new CookieOptions
        {
            Path = "/",
            MaxAge = TimeSpan.FromDays(30), // 30 dias
            Secure = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production",
            HttpOnly = true,
            SameSite = SameSiteMode.Lax
        };

        Response.Cookies.Append("session_id", sessionToken, cookieOptions);
    }
}