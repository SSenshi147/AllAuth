using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace AllAuth.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet("tokens")]
    public async Task<List<TokenResponse>> Tokens()
    {
        _logger.LogInformation("tokens called");
        var token = await this.HttpContext.GetTokenAsync("access_token");
        _logger.LogInformation("token: {token}", token);
        var tokens = HttpContext.User.Claims.Select(x => new TokenResponse() { Type = x.Type, Value = x.Value })
            .ToList();
        foreach (var tokenResponse in tokens)
        {
            _logger.LogInformation("tokens: {type}, {value}", tokenResponse.Type, tokenResponse.Value);
        }
        return tokens;
    }

    [HttpGet("login")]
    public async Task<IResult> Login()
    {
        _logger.LogInformation("login called");
        return Results.Challenge(new AuthenticationProperties()
        {
            RedirectUri = "https://localhost:5005/WeatherForecast/tokens"
        }, authenticationSchemes: new List<string>() { "github" });
    }
}

public class TokenResponse
{
    public string Type { get; set; }
    public string Value { get; set; }
}