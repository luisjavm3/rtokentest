using AutoMapper;
using rtoken.api.DTOs.User;
using rtoken.api.Models.Entities;

namespace rtoken.api
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserResponse>();
        }
    }
}