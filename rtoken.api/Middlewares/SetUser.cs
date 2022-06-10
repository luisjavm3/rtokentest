using System.Security.Claims;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using rtoken.api.Data;
using rtoken.api.DTOs.User;
using rtoken.api.Models.Entities;

namespace rtoken.api.Middlewares
{
    public class SetUser
    {
        private readonly RequestDelegate _next;
        public SetUser(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, DataContext dataContext, IMapper mapper)
        {
            try
            {
                int userId = int.Parse(context.User.FindFirstValue(claimType: "id"));
                User user = await dataContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
                context.Items["User"] = mapper.Map<UserResponse>(user);
            }
            catch (System.Exception)
            {
                context.Items["User"] = null;
            }

            await _next(context);
        }
    }
}