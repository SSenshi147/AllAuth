using System.Security.Claims;
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
    public async Task<IActionResult> Tokens([FromQuery] string redirect)
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
        
        Response.Cookies.Append("asd", "gec");

        var withparams = $"{redirect}?id={DataHolder.Data.Id}&name={DataHolder.Data.Login}";
        return Redirect(withparams);
    }

    [HttpGet("login")]
    public async Task<IResult> Login([FromQuery] string redirect)
    {
        _logger.LogInformation("login called");

        var result = Results.Challenge(new AuthenticationProperties()
        {
            RedirectUri = $"/WeatherForecast/tokens?redirect={redirect}"
        }, authenticationSchemes: new List<string>() { "github" });

        ;

        return result;
    }
}

public class TokenResponse
{
    public string Type { get; set; }
    public string Value { get; set; }
}