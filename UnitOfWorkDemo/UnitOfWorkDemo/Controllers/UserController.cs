using DataAccess.Interfaces;
using DataAccessWithEF.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UnitOfWorkDemo.Dtos;

namespace UnitOfWorkDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("CreateUser")]
        [ProducesResponseType(typeof(UserResponseDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateUser(CreateUserDto createUser)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                User user = new User()
                {
                    Name = createUser.UserName,
                    Email = createUser.UserEmail,
                    DOB = createUser.DOB,
                    ContactNumber = createUser.ContactNumber,
                    CreatedTime = DateTimeOffset.Now,
                    LastEditedTime = DateTimeOffset.Now
                };

                await _unitOfWork.UserRepo.AddAsync(user);

                await _unitOfWork.SaveChnagesAsync();
                await _unitOfWork.CompleteTransactionAsync();

                return Ok(new UserResponseDto()
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserName = user.Name
                });
            }
            catch (Exception ex) 
            { 
                await _unitOfWork.RollbackTransactionAsync();
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("DeleteUser")]
        [ProducesResponseType(typeof(UserResponseDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                User? user = await _unitOfWork
                .UserRepo
                .GetByIdAsync(userId);

                if (user is not null)
                {
                    _unitOfWork.UserRepo.Delete(user);

                    await _unitOfWork.SaveChnagesAsync();
                    await _unitOfWork.CompleteTransactionAsync();

                    return Ok(new UserResponseDto()
                    {
                        UserId = user.Id,
                        UserEmail = user.Email,
                        UserName = user.Name
                    });
                }
                else
                {
                    return Ok();
                }
            }
            catch (Exception ex) 
            {
                await _unitOfWork.BeginTransactionAsync();
                return StatusCode(500, ex.Message);
            }
        }
    }
}
