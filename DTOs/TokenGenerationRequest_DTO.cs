using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace UserService.DTOs;

public class TokenGenerationRequest_DTO
{
    [EmailAddress]
    public string Email { get; set; }
    public Guid UserId { get; set; }

    // Optional custom claims
    public Dictionary<string, JsonElement> CustomClaims { get; set; }
}
