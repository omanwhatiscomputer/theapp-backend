using System;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using UserService.Data;
using UserService.DTOs;
using UserService.Entities;
using UserService.RequestHelpers;

namespace UserService.Controllers;

[ApiController]
[Route("api/users")]
public class UserController(UserDbContext context, IMapper mapper, IConfiguration config) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<User_DTO>>> GetAllUsers()
    {
        var query = context.Users.OrderByDescending(x => x.LastLogin).AsQueryable();
        var users =  await query.ProjectTo<User_DTO>(mapper.ConfigurationProvider).ToListAsync();
        return Ok(users);
    }



    [AllowAnonymous]
    [HttpPost("auth")]
    public async Task<ActionResult<User_DTO>> Login(LogIn_DTO logIn_dto)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Email == logIn_dto.Email);
        if(user == null) return NotFound("User not found!");
        if (!PasswordHelper.HashesMatch(logIn_dto.Password, user.PasswordSalt, user.Password))
        {
            return Unauthorized("Invalid Credentials");
        }
        
        string token = UserHelper.GetUserToken(user, config);

        if (!PasswordHelper.PasswordSaltIsValid(user.SaltLastRenewedAt, config)){
            byte[] newSalt;
            string newPasswordHash = PasswordHelper.HashPassword(logIn_dto.Password, out newSalt);
            user.PasswordSalt = PasswordHelper.SaltConvertBytes2HexString(newSalt);
            user.Password = newPasswordHash;
            user.SaltLastRenewedAt = DateTime.UtcNow;
        }

        user.LastLogin = DateTime.UtcNow;
        user.PreviousLoginDates.Add(DateOnly.FromDateTime(user.LastLogin));

        var result = await context.SaveChangesAsync() > 0;
        if(!result) return BadRequest("Could not save changes to the DB.");

        UserHelper.AppendCookies(Response, config, user, token);

        var user_dto = mapper.Map<User_DTO>(user);
        user_dto.Token = token;

        return Ok(user_dto);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<User_DTO>> GetUserById(Guid id)
    {
        

        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
        if(user == null) return NotFound();

        var user_dto = mapper.Map<User_DTO>(user);
        return Ok(user_dto);

    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<User_DTO>>CreateUser(CreateUser_DTO createU_dto)
    {

        // if(await DataValidationHelper.SignUpEmailExists(context, createU_dto.Email))
        // {
        //     return BadRequest("Email is already in use.");
        // }
        byte[] salt;
        string passwordHash = PasswordHelper.HashPassword(createU_dto.Password, out salt);

        var user = mapper.Map<User>(createU_dto);
        // while(await DataValidationHelper.SignUpIdExists(context, user.Id))
        // {
        //     user = mapper.Map<User>(createU_dto);
        // }
        user.PasswordSalt = PasswordHelper.SaltConvertBytes2HexString(salt);
        user.Password = passwordHash;


        context.Users.Add(user);
        // try{
        //     var result = await context.SaveChangesAsync() > 0;
        //     if(!result) return StatusCode(500, "Could not save changes to the DB.");
        // }catch(NpgsqlException exception){
        //     return BadRequest("Email is already in use.");
        // }
        try
        {
            var result = await context.SaveChangesAsync() > 0;
            if (!result) 
                return StatusCode(500, "Could not save changes to the DB.");
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
            {
                return BadRequest("Email is already in use.");
            }
            
            return StatusCode(500, "An unexpected error occurred while saving changes.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
        


        string token = UserHelper.GetUserToken(user, config);

        UserHelper.AppendCookies(Response, config, user, token);
        var user_dto = mapper.Map<User_DTO>(user);
        user_dto.Token = token;
        return CreatedAtAction(nameof(Login), new {user.Id}, user_dto);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult<User_DTO>> DeleteUserById(Guid id)
    {

        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
        if(user == null) return NotFound();

        context.Users.Remove(user);
        var result = await context.SaveChangesAsync() > 0;
        if(!result) return BadRequest("Error occurred while deleting user.");
        var user_dto = mapper.Map<User_DTO>(user);

        return Ok(user_dto);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<User_DTO>> UpdateUserById(Guid id, UpdateUser_DTO updateUser_dto)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Id ==id);
        if(user == null) return NotFound();

        mapper.Map<UpdateUser_DTO, User>(updateUser_dto, user);
        var result = await context.SaveChangesAsync() > 0;


        if (result) return Ok(mapper.Map<User_DTO>(user));
        
        return BadRequest("Problem saving changes");

    }
}
