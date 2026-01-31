using Chinese_Auction;
using Chinese_Auction.Dto_s;
using Chinese_Auction.Services;
using ChineseAuction.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChineseAuction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userSevice;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userSevice, ILogger<UserController> logger)
        {
            _userSevice = userSevice;
            _logger = logger;
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            _logger.LogInformation("Getting all users.");
            var users = await _userSevice.GetAllUsers();
            _logger.LogInformation("Fetched all users successfully.");
            return Ok(users);
        }

        [HttpGet("example")]
        public async Task<IActionResult> ExampleGetAllUsers()
        {
            _logger.LogInformation("Getting all users - example endpoint.");
            var users = await _userSevice.GetAllUsers();
            _logger.LogInformation("Fetched all users successfully - example endpoint.");
            return Ok(users);
        }

        [Authorize]
        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<IActionResult> GetUserByIdAsync(int id)
        {
            _logger.LogInformation($"Getting user with ID: {id}");
            if (!User.IsManager() && User.GetUserId() != id)
            {
                return Forbid();
            }
            var user = await _userSevice.GetUserByIdAsync(id);
            _logger.LogInformation($"Fetched user with ID: {id} successfully.");
            return user == null ? NotFound() : Ok(user);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserAsync(int id, [FromBody] CreateUserDto user)
        {
            _logger.LogInformation($"Updating user with ID: {id}");
            if (User.IsManager() || User.GetUserId() != id)
            {
                return Forbid();
            }

            try
            {
                var updatedUser = await _userSevice.UpdateUser(id, user);
                _logger.LogInformation($"Updated user with ID: {id} successfully.");
                return updatedUser == null ? NotFound() : Ok(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user with ID: {id}");
                return BadRequest("Internal server error occurred");
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            _logger.LogInformation($"Deleting user with ID: {id}");
            if (User.IsManager() || User.GetUserId() != id)
            {
                return Forbid();
            }
            try
            {
                await _userSevice.DeleteUser(id);
                _logger.LogInformation($"Deleted user with ID: {id} successfully.");
                return Ok("deleted succesfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user with ID: {id}");
                return BadRequest("Internal server error occurred");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync([FromBody] CreateUserDto userDto)
        {
            _logger.LogInformation("Registering new user.");
            try
            {
                var newUser = await _userSevice.CreateUser(userDto);
                if (newUser != null)
                {
                    _logger.LogInformation("User registered successfully.");
                    return CreatedAtRoute("GetUserById", new { id = newUser.Id }, newUser);
                }

                return BadRequest("User could not be created.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering new user.");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUserAsync([FromBody] LoginRequestDto userDto)
        {
            _logger.LogInformation("Authenticating user.");
            try
            {
                var authResponse = await _userSevice.AuthenticateAsync(userDto);
                _logger.LogInformation("User authentication process completed.");
                return authResponse == null ? Unauthorized() : Ok(authResponse);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error during user authentication.");
                return BadRequest(ex.Message);
            }
            
        }
    }
}