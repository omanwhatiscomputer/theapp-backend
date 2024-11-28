using System;
using AutoMapper;
using UserService.DTOs;
using UserService.Entities;

namespace UserService.RequestHelpers;

public class MappingProfiles : Profile
{
    public MappingProfiles(){
        // get users/user
        CreateMap<User, User_DTO>();
        // create user
        CreateMap<CreateUser_DTO, User>();
        // update user
        CreateMap<UpdateUser_DTO, User>()
            .ForMember(d => d.LastLogin, o=> o.Ignore())
            .ForMember(d => d.CreatedAt, o=> o.Ignore())
            .ForMember(d => d.PreviousLoginDates, o=> o.Ignore())
            .ForMember(d => d.SaltLastRenewedAt, o=> o.Ignore())
            .ForAllMembers(options => options.Condition((src, dest, srcValue) => srcValue != null));

    }
}
