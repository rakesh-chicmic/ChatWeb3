using ChatWeb3.Data;
using ChatWeb3.Models;
using ChatWeb3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatWeb3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //controller for user details, updating user and deleting user
        IUserService _userService;                   //service dependency
        Response response = new Response();
        private readonly ILogger<UserController> _logger;

        public UserController(IConfiguration configuration, ChatAppDbContext dbContext, ILogger<UserController> logger, IUserService userService)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet, Authorize]
        [Route("/api/v1/users/getYourself")]
        public IActionResult GetYourself()                  // api for user to get data of himself for proifile details
        {
            _logger.LogInformation("Get your self details method started");
            try
            {
                string id = User.FindFirstValue(ClaimTypes.PrimarySid)!;
                response = _userService.GetYourself(id);
                return StatusCode(response.statusCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Internal server error ", ex.Message);
                return StatusCode(500, $"Internal server error: {ex}"); ;
            }
        }

        //getusers api to get list of other users and details
        [HttpGet, Authorize]
        [Route("/api/v1/users/get")]
        public IActionResult GetUsers(string? searchString = null, string? accountAddress = null, string ? id = null, String OrderBy = "username", int SortOrder = 1, int RecordsPerPage = 20, int PageNumber = 0)          // sort order   ===   e1 for ascending  -1 for descending
        {
            _logger.LogInformation("Get list of users method started");
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.PrimarySid)!;
                response = _userService.GetUsers(userId,searchString,accountAddress,id, OrderBy, SortOrder, RecordsPerPage, PageNumber);
                return StatusCode(response.statusCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Internal server error ", ex.Message);
                return StatusCode(500, $"Internal server error: {ex}"); ;
            }
        }

        [HttpPut ,Authorize]
        [Route("/api/v1/users/registerUpdate")]
        public IActionResult RegisterUpdateStudent(UpdateUser update)
        {
            _logger.LogInformation("Update user method started");
            try
            {
                string id = User.FindFirstValue(ClaimTypes.PrimarySid)!;
                response = _userService.UpdateUser(id,update).Result;
                return StatusCode(response.statusCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Internal server error ", ex.Message);
                return StatusCode(500, $"Internal server error: {ex}"); ;
            }
        }

        [HttpPut, Authorize]
        [Route("/api/v1/users/validateUsername")]
        public IActionResult ValidateUsername(string username)
        {
            _logger.LogInformation("Validate username method started");
            try
            {
                string id = User.FindFirstValue(ClaimTypes.PrimarySid)!;
                response = _userService.ValidateUsername(id, username);
                return StatusCode(response.statusCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Internal server error ", ex.Message);
                return StatusCode(500, $"Internal server error: {ex}"); ;
            }
        }

        [HttpDelete, Authorize]
        [Route("/api/v1/user/delete")]
        public IActionResult DeleteUserAccount()
        {
            _logger.LogInformation("Delete user method started");
            try
            {
                string id = User.FindFirstValue(ClaimTypes.PrimarySid)!;
                response = _userService.DeleteUser(id).Result;
                return StatusCode(response.statusCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Internal server error ", ex.Message);
                return StatusCode(500, $"Internal server error: {ex}"); ;
            }
        }
    }
}
