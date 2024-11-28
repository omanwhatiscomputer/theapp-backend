using System;
using UserService.Entities;

namespace UserService.DTOs;

public class UpdateUser_DTO
{
    public DateTime? LastLogin {get; set;}
    #nullable enable
    public List<DateOnly>? PreviousLoginDates{get; set;}
    public string? PasswordSalt {get; set;}
    #nullable disable
    public Status AccountStatus {get; set;}
    public DateTime? SaltLastRenewedAt {get; set;}
}
