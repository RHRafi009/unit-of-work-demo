using DataAccessWithEF;
using DataAccessWithEF.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnitOfWorkDemo.Dtos;

namespace UnitOfWorkDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly UnitOfWorkDemoDbContext _unitOfWorkDemoDbContext;

        public UserController(UnitOfWorkDemoDbContext unitOfWorkDemoDbContext)
        {
            _unitOfWorkDemoDbContext = unitOfWorkDemoDbContext;
        }

        [HttpPost("CreateUser")]
        public async Task<UserResponseDto> CreateUser(CreateUserDto createUser)
        {
            User user = new User()
            {
                Name = createUser.UserName,
                Email = createUser.UserEmail,
                DOB = createUser.DOB,
                ContactNumber = createUser.ContactNumber,
                CreatedTime = DateTimeOffset.Now,
                LastEditedTime = DateTimeOffset.Now
            };

            _unitOfWorkDemoDbContext.Users.Add(user);

            await _unitOfWorkDemoDbContext.SaveChangesAsync();

            return new UserResponseDto()
            {
                UserId = user.Id,
                UserEmail = user.Email,
                UserName = user.Name
            };
        }

        [HttpDelete("DeleteUser")]
        public async Task<UserResponseDto?> DeleteUser(int userId)
        {
            User? user = await _unitOfWorkDemoDbContext
                .Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user is not null)
            {
                _unitOfWorkDemoDbContext.Users.Remove(user);

                await _unitOfWorkDemoDbContext.SaveChangesAsync();

                return new UserResponseDto()
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserName = user.Name
                };
            }
            else 
            { 
                return null; 
            }
        }
    }
}
