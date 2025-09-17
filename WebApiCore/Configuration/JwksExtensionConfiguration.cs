using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApiCore.Extensions;
using WebApiCore.Secutiry;

namespace WebApiCore.Configuration;

public static class JwksExtensionConfiguration
{
    public static void AddJwtConfiguration(this IServiceCollection Services, IConfiguration Configuration)
    {
        var appSettingsSection = Configuration.GetSection(key: "AppSettings");

        Services.Configure<AppSettings>(appSettingsSection);

        var appSettings = appSettingsSection.Get<AppSettings>();
        Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = true;
            x.SaveToken = true;
            x.SetJwksOptions(new (appSettings?.AutenticacaoJwksUrl));

            //x.TokenValidationParameters = new TokenValidationParameters
            //{
            //    ValidateIssuer = true,
            //    ValidIssuer = "localhost",

            //    ValidateAudience = true,
            //    ValidAudiences = ["api.authentication"],   // suporta 1..N audiences

            //    ValidateLifetime = true,
            //    RequireExpirationTime = true,
            //    ClockSkew = TimeSpan.FromSeconds(30),

            //    ValidateIssuerSigningKey = true,
            //    RequireSignedTokens = true,
            //    IssuerSigningKey = new SymmetricSecurityKey(
            //Encoding.UTF8.GetBytes("super-segredo-de-desenvolvimento-min-32-bytes!!!!"))
            //};

            x.Events = new()
            {
                OnMessageReceived = ctx =>
                {
                    var hasAuth = ctx.Request.Headers.ContainsKey("Authorization");
                    var auth = hasAuth ? ctx.Request.Headers["Authorization"].ToString() : "(sem header)";
                    ctx.HttpContext.RequestServices
                       .GetRequiredService<ILoggerFactory>()
                       .CreateLogger("JWT")
                       .LogInformation("Authorization header: {auth}", auth);
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = ctx =>
                {
                    ctx.HttpContext.RequestServices
                           .GetRequiredService<ILoggerFactory>()
                           .CreateLogger("JWT")
                           .LogError(ctx.Exception, "Falha na autenticação JWT");
                    return Task.CompletedTask;
                },
                OnTokenValidated = ctx =>
                {
                    var logger = ctx.HttpContext.RequestServices
                            .GetRequiredService<ILoggerFactory>()
                            .CreateLogger("JWT");

                    var sub = ctx.Principal?.FindFirst("sub")?.Value;
                    var auds = string.Join(",", ctx.Principal?.Claims.Where(c => c.Type == "aud").Select(c => c.Value) ?? Array.Empty<string>());
                    var email = ctx.Principal?.FindFirst("email")?.Value;
                    logger.LogInformation("Token válido. sub={sub}, email={email}, aud={aud}", sub, email, auds);
                    return Task.CompletedTask;
                },
                OnChallenge = ctx =>
                {
                    ctx.HandleResponse();
                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    var err = new
                    {
                        error = ctx.Error,
                        error_description = ctx.ErrorDescription,
                        error_uri = ctx.ErrorUri
                    };
                    return ctx.Response.WriteAsJsonAsync(err);
                }
            };
        });
    }

    public static void SetJwksOptions(this JwtBearerOptions options, JwkOptions jwkOptions)
    {
        // TODO: Implementar aqui a configuração do JWT Bearer usando as opções do JWK
    }
}
