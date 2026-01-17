using AutoMapper;
using Chinese_Auction.Dto_s;
using Chinese_Auction.Models;
using Chinese_Auction.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System.Drawing;
namespace Chinese_Auction.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private IConfiguration _configuration;
        private IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IConfiguration configuration, ITokenService tokenService, ILogger<UserService> logger, IMapper mapper)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _tokenService = tokenService;
            _logger = logger;
            _mapper = mapper;
        }


        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsers();
            return _mapper.Map<IEnumerable<User>>(users);
        }


        public async Task<User?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null) 
            { 
                _logger.LogWarning("User with ID {UserId} not found.", id);
                return null; 
            }
            return _mapper.Map<User>(user);
        }


        public async Task<GetUserDto?> CreateUser(CreateUserDto user)
        {
            if (await EmailExistsAsync(user.Email, -1))
            {
                _logger.LogWarning("Attempt to create user with existing email: {Email}", user.Email);
                throw new Exception("User with the same email already exists.");
            }
            user.Password = HashPassword(user.Password);
            var createUser = _mapper.Map<User>(user);
            await _userRepository.CreateUser(createUser);
            return _mapper.Map<GetUserDto>(createUser);
        }

        private static string HashPassword(string password)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }

        public async Task<GetUserDto?> UpdateUser(int id, CreateUserDto user)
        {
            var existingUser = await _userRepository.GetUserById(id);
            if (existingUser == null)
            {
                _logger.LogWarning("Attempt to update non-existing user with ID: {UserId}", id);
                return null;
            }
            _mapper.Map(user, existingUser);
            if (user.Email != existingUser.Email && user.Email != null)
            {
                if (await EmailExistsAsync(user.Email, id))
                {
                    _logger.LogWarning("Attempt to update user with existing email: {Email}", user.Email);
                    throw new Exception("User with the same email already exists.");
                }
            }
            if (user.Password != null) 
                existingUser.Password = HashPassword(user.Password);
            existingUser.Id = id;
            var updatedUser = await _userRepository.UpdateUser(existingUser);
            if(updatedUser != null) {
                _logger.LogInformation("User with ID {UserId} updated successfully.", id);
                return null;
            }
            return _mapper.Map<GetUserDto>(updatedUser);
        }


        public async Task<bool> DeleteUser(int id)
        {
            var existingUser = await _userRepository.GetUserById(id);
            if (existingUser == null)
            {
                _logger.LogWarning("Attempt to delete non-existing user with ID: {UserId}", id);
                return false;
            }
            await _userRepository.DeleteUserAsync(id);
            return true;
        }

        private async Task<bool> EmailExistsAsync(string email, int id)
        {
            return await _userRepository.EmailExistsAsync(email, id);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _userRepository.GetUserByEmail(email);
        }

        public async Task<LoginResponseDto?> AuthenticateAsync(LoginRequestDto loginRequest)
        {
            var user = await _userRepository.GetUserByEmail(loginRequest.Email);
            if (user == null)
            {
                _logger.LogWarning("Login attempt failed: User not found for email {Email}", loginRequest.Email);
                return null;
            }
            var hashedPassword = HashPassword(loginRequest.Password);
            if (user.Password != hashedPassword)
            {
                _logger.LogWarning("Login attempt failed: Invalid password for email {Email}", loginRequest.Email);
                return null;
            }

            var token = _tokenService.GenerateToken(user.Id, user.Email, user.First_name, user.Last_name, user.Phone, user.Role);
            var expiryMinutes = _configuration.GetValue<int>("JwtSettings:ExpiryMinutes", 60);

            _logger.LogInformation("User {UserId} authenticated successfully", user.Id);

            return new LoginResponseDto
            {
                Token = token,
                TokenType = "Bearer",
                ExpiresInMinutes = expiryMinutes * 60,
                User = _mapper.Map<GetUserDto>(user)
            };
        }

        public async Task<IEnumerable<User>> GetFilteredUsersAsync(string? name, string? email)
        {
            return await _userRepository.GetFilteredUsersAsync(name, email);
        }





    }
}
