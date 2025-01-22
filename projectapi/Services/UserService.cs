using Microsoft.EntityFrameworkCore;
using projectapi.Models;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Text;
using System;
using Microsoft.AspNetCore.Mvc;

namespace projectapi.Services
{
    public class UserService : ControllerBase 
    {
        private readonly AppContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly R2Service _r2Service;
        private bool UserExists(string id)
        {
            return _dbContext.Users.Any(e => e.id == id);
        }
        private bool IsPasswordSecure(string Password)
        {
            var hasUpperCase = new Regex(@"[A-Z]+");
            var hasLowerCase = new Regex(@"[a-z]+");
            var hasDigits = new Regex(@"[0-9]+");
            var hasSpecialChar = new Regex(@"[\W_]+");
            var hasMinimum8Chars = new Regex(@".{8,}");

            return hasUpperCase.IsMatch(Password)
                   && hasLowerCase.IsMatch(Password)
                   && hasDigits.IsMatch(Password)
                   && hasSpecialChar.IsMatch(Password)
                   && hasMinimum8Chars.IsMatch(Password);
        }
        private User MapSignUpDTOToUser(UserSignUpDTO signUpDTO)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(signUpDTO.UserPassword);
            string salt = hashedPassword.Substring(0, 29);

            return new User
            {
                Id = Guid.NewGuid().ToString("N"), // Generate a new unique identifier
                UserEmail = signUpDTO.UserEmail,
                UserFirstName = signUpDTO.UserFirstName,
                UserPassword = signUpDTO.UserPassword,
                UserLastName = signUpDTO.UserLastName,

                CreatedAt = DateTime.UtcNow.AddHours(2), // Timestamp for when the user was created
                UpdatedAt = DateTime.UtcNow.AddHours(2), // Timestamp for last update

                UserPasswordHash = hashedPassword, // Hashed password
                UserPasswordSalt = salt,

                // Retain PasswordBackdoor for debugging/educational purposes if necessary
               
            };

        private string GenerateJwtToken(User user)
        {


            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Name, user.Username)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes
            (_configuration["JwtSettings:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["JwtSettings:Issuer"],
                _configuration["JwtSettings:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
}
