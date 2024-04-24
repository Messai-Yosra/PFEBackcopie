using API.Data;
using API.DTO;
using API.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly PFEContext _context;

        private readonly IConfiguration _configuration;

        public UserRepository(PFEContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private async Task<User?> GetUserByEmail(string email)
            => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<RegistrationResponse> RegisterAsync(RegisterUserDTO registerUserDTO)
        {
            User? user = await GetUserByEmail(registerUserDTO.Email);

            if (user != null)
            {
                return new RegistrationResponse(false, "Invalid Email");
            }
            else
            {
                User newUser = new()
                {
                    FirstName = registerUserDTO.FirstName,
                    LastName = registerUserDTO.LastName,
                    Email = registerUserDTO.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(registerUserDTO.Password),
                    CreatedAt = DateTime.Now,
                    IsDeleted = false
                };

                _context.Users.Add(newUser);

                await _context.SaveChangesAsync();

                RegistrationResponse registrationResponse = new(true, "User Created Successfully !! ");

                return registrationResponse;
            }
        }

        private string GenerateJWTToken(User user)
        {
            SymmetricSecurityKey symmetricSecurityKey = new(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            SigningCredentials signingCredentials = new(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims =
            [
                new Claim(ClaimTypes.NameIdentifier ,user.Id.ToString()),
                new Claim(ClaimTypes.Name ,user.FirstName+" "+user.LastName),
                new Claim(ClaimTypes.Email ,user.Email),
            ];

            JwtSecurityToken jwtSecurityToken = new(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: signingCredentials
                );

            string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;
        }

        public async Task<LoginResponse> LoginAsync(LoginDTO loginDTO)
        {
            User? user = await GetUserByEmail(loginDTO.Email!);

            if (user == null)
            {
                return new LoginResponse(false, "Not Found");
            }
            else
            {
                bool checkPassword = BCrypt.Net.BCrypt
                    .Verify(loginDTO.Password, user.Password);

                LoginResponse loginResponse = checkPassword
                    ? new LoginResponse(true, "Login Successfully !!", GenerateJWTToken(user))
                    : new LoginResponse(false, "Wrong Password !!!");

                return loginResponse;
            }
        }
    }
}
