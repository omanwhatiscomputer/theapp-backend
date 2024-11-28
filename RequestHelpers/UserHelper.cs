using System;
using System.Text.Json;
using UserService.DTOs;
using UserService.Entities;

namespace UserService.RequestHelpers;

public class UserHelper
{
    public static string GetUserToken(User user, IConfiguration config)
    {
        var tokenRequest = new TokenGenerationRequest_DTO
        {
            Email = user.Email,
            UserId = user.Id,
            CustomClaims = new Dictionary<string, JsonElement>{
                { "admin", JsonDocument.Parse("true").RootElement }// see identity constants
            }
        };
        TimeSpan tokenLifetime = TimeSpan.FromHours(Convert.ToDouble(config["JwtSettings:DefaultTokenLifetime"]));
        string token = JwtHelper.GenerateJwtToken(tokenRequest, config, tokenLifetime);
        return token;
    }

    public static void AppendCookies(HttpResponse response, IConfiguration config, User user, string token)
    {
        double cookieLifetime = Convert.ToDouble(config["CookieLifetimeInHours"]);

        response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddHours(cookieLifetime)
        });

        response.Cookies.Append("userId", user.Id.ToString(), new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddHours(cookieLifetime)
        });
    }
}
