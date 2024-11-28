using System;
using Microsoft.EntityFrameworkCore;
using UserService.Data;

namespace UserService.RequestHelpers;

public class DataValidationHelper
{
    public static async Task<bool> SignUpEmailExists(UserDbContext context, string candidateEmail)
    {
        var res = await context.Users.FirstOrDefaultAsync(x => x.Email.Equals(candidateEmail.ToLower()));
        
        if(res != null) return true;
        return false;
    }
    public static async Task<bool> SignUpIdExists(UserDbContext context, Guid candidateId)
    {
        var res = await context.Users.FirstOrDefaultAsync(x => x.Id.CompareTo(candidateId) == 0);
        if(res != null) return true;
        return false;
    }
}
