using System;
using System.ComponentModel.DataAnnotations;

namespace UserService.DTOs;

public class CreateUser_DTO
{
    [Required]
    [EmailAddress]
    public string Email {get; set;}
    [Required]
    [StringLength(512, MinimumLength = 1)]
    public string Password {get; set;}
    [Required]
    public string FirstName {get; set;}
    [Required]
    public string LastName {get; set;}
    public string Company {get; set;} = string.Empty;
    public string Designation {get; set;} = string.Empty;
}
