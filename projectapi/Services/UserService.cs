using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using projectapi.Models;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Text;
using System;


using Microsoft.AspNetCore.Mvc;
using projectapi.Context;

namespace projectapi.Services
{
    public class UserService : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UserService(ApplicationDbContext _context, IConfiguration configuration)
        {
            this._context = _context;
            _configuration = configuration;
            //_accessKey = config.AccessKey;
            //_secretKey = config.SecretKey;
            //_r2Service = new R2Service(_accessKey, _secretKey);
        }

        public bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
        public bool IsPasswordSecure(string Password)
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
        public User MapSignUpDTOToUser(UserSignUpDTO signUpDTO)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(signUpDTO.UserPassword);
            string salt = hashedPassword.Substring(0, 29);

            return new User
            {
                Id = Guid.NewGuid().ToString("N"),
                UserEmail = signUpDTO.UserEmail,
                UserFirstName = signUpDTO.UserFirstName,
                UserPassword = signUpDTO.UserPassword,
                UserLastName = signUpDTO.UserLastName,

                CreatedAt = DateTime.UtcNow.AddHours(2), 
                UpdatedAt = DateTime.UtcNow.AddHours(2), 

                UserPasswordHash = hashedPassword, 
                UserPasswordSalt = salt,

               

            };
        }

        public string GenerateJwtToken(User user)
        {


            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Name, user.UserFirstName)
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

