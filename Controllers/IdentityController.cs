
using Microsoft.AspNetCore.Mvc;

using UserService.DTOs;
using UserService.RequestHelpers;

namespace UserService.Controllers;

public class IdentityController(IConfiguration config) : ControllerBase
{
    
    
    [HttpPost("api/token")]
    public IActionResult GenerateToken([FromBody] TokenGenerationRequest_DTO request)
    {
        TimeSpan tokenLifetime = TimeSpan.FromHours(Convert.ToDouble(config["JwtSettings:DefaultTokenLifetime"]));
        var jwt = JwtHelper.GenerateJwtToken(request, config, tokenLifetime);
        return Ok(jwt);
        
    }
}
