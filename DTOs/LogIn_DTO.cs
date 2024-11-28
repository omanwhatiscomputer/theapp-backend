using System;
using System.ComponentModel.DataAnnotations;

namespace UserService.DTOs;

public class LogIn_DTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
