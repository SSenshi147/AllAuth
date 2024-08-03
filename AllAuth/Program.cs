using System.Net.Http.Headers;
using System.Text.Json;
using AllAuth.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AllAuth;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors();
        builder.Services
            .AddAuthentication("cookie")
            .AddCookie("cookie")
            .AddOAuth("github", config =>
            {
                // https://github.com/settings/applications/2659856
                config.ClientId = "Ov23lizKZov1naH8oY12";
                config.ClientSecret = "dd7fcc3a778f66b930f1938159593e9d88fc5c75";

                // random
                config.CallbackPath = "/github/callback";

                // https://docs.github.com/en/apps/oauth-apps/building-oauth-apps/authorizing-oauth-apps#1-request-a-users-github-identity
                config.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";

                // https://docs.github.com/en/apps/oauth-apps/building-oauth-apps/authorizing-oauth-apps#2-users-are-redirected-back-to-your-site-by-github
                config.TokenEndpoint = "https://github.com/login/oauth/access_token";

                config.SaveTokens = true;

                // https://docs.github.com/en/apps/oauth-apps/building-oauth-apps/authorizing-oauth-apps#3-use-the-access-token-to-access-the-api
                config.UserInformationEndpoint = "https://api.github.com/user";

                config.Events.OnCreatingTicket += async context =>
                {
                    using var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                    using var response = await context.Backchannel.SendAsync(request);
                    var user = await response.Content.ReadFromJsonAsync<JsonElement>();
                    context.RunClaimActions(user);
                };
            });

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        // builder.Services.AddEndpointsApiExplorer();
        // builder.Services.AddSwaggerGen();

        var app = builder.Build();

        //app.UseCors(x => x.AllowAnyOrigin());
        // Configure the HTTP request pipeline.
        // if (app.Environment.IsDevelopment())
        // {
        //     app.UseSwagger();
        //     app.UseSwaggerUI();
        // }

        app.UseAuthentication();

        // app.MapGet("/", async (HttpContext context, ILogger logger) =>
        // {
        //     await context.GetTokenAsync("access_token");
        //     var tokens = context.User.Claims.Select(x => new TokenResponse() { Type = x.Type, Value = x.Value })
        //         .ToList();
        //     return tokens;
        // });
        //
        // app.MapGet("/login", () => Results.Challenge(new AuthenticationProperties()
        // {
        //     RedirectUri = "https://localhost:5005/"
        // }, authenticationSchemes: new List<string>() { "github" }));

        //app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}