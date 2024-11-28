using System;
using Microsoft.EntityFrameworkCore;


namespace UserService.Entities;

[PrimaryKey(nameof(Email), nameof(Id))]
public class User
{
    public Guid Id {get; set;} = Guid.NewGuid();
    public required string Email {get; set;}
    public required string Password {get; set;}
    public required string FirstName {get; set;}
    public required string LastName {get; set;}
    public string Company {get; set;}
    public string Designation{get; set;}
    public DateTime LastLogin{get; set;} = DateTime.UtcNow;
    public DateTime CreatedAt{get; set;} = DateTime.UtcNow;
    public List<DateOnly> PreviousLoginDates{get; set;} = [DateOnly.FromDateTime(DateTime.Now)];
    public Status AccountStatus {get; set;} = Status.Active;
    public string PasswordSalt {get; set;}
    public DateTime SaltLastRenewedAt {get; set;} = DateTime.UtcNow;
}
