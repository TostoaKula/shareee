using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projectapi.Context;
using projectapi.Models;
using System;
using projectapi.Services;

namespace projectapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly UserService _userService;
        // private readonly R2Service _r2Service;

        public UserController
            (
            ApplicationDbContext dbContext, 
            IConfiguration configuration, 
            UserService userService

            )

        {
            _userService = userService;
            _dbContext = dbContext;
            _configuration = configuration;
            //_accessKey = config.AccessKey;
            //_secretKey = config.SecretKey;
            //_r2Service = new R2Service(_accessKey, _secretKey);
        }
        [HttpPost("SignUp")]
        public async Task<IActionResult> PostUser([FromForm] UserSignUpDTO userSignUp)
        {
            
            if (await _dbContext.Users.AnyAsync(u => u.UserName == userSignUp.UserName))
            {
                return Conflict(new { message = "Username is already in use." });
            }

            if (await _dbContext.Users.AnyAsync(u => u.UserEmail == userSignUp.UserEmail))
            {
                return Conflict(new { message = "Email is already in use." });
            }

            if (!_userService.IsPasswordSecure(userSignUp.UserPassword))
            {
                return Conflict(new { message = "Password is not secure." });
            }

            var user = _userService.MapSignUpDTOToUser(userSignUp);

            //var r2Service = new R2Service(_accessKey, _secretKey);
            //var imageUrl = await r2Service.UploadToR2(userSignUp.ProfilePicture.OpenReadStream(), "PP" + user.id);
            //user.ProfilePicture = imageUrl;

            _dbContext.Users.Add(user);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (_userService.UserExists(user.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.UserAdress,
                user.UserFirstName,
                user.UserEmail,
                user.UserLastName,
               
            });
        }
        // login
        [HttpPost("login")]

        public async Task<IActionResult> Login(UserLoginDTO login)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.UserEmail == login.UserEmail);
            if (user == null || !BCrypt.Net.BCrypt.Verify(login.UserPassword, user.UserPasswordHash))
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }
            var token = _userService.GenerateJwtToken(user);
            return Ok(new { token, user.UserName, user.Id });
        }
        
        
    }
}
