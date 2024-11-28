

using System;

using UserService.Entities;


namespace UserService.DTOs;

public class User_DTO
{
    public Guid Id {get; set;}
    public string Email {get; set;}
    public string FirstName {get; set;}
    public string LastName {get; set;}
    public string Company {get; set;}
    public string Designation {get; set;}
    public DateTime LastLogin {get; set;}
    public List<DateOnly> PreviousLoginDates{get; set;}
    public Status AccountStatus {get; set;}
    public DateTime SaltLastRenewedAt {get; set;}
    #nullable enable
    public string? Token {get; set;}
    #nullable disable
}
