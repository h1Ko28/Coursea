using Coursea.Data;
using Coursea.Dto.Account;
using Coursea.Interfaces;
using Coursea.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EmailManagement.Services;
using EmailManagement.Models;
using Microsoft.AspNetCore.Authorization;

namespace Coursea.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;
        private readonly DataContext _dataContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<User> userManager, ITokenService tokenService, 
            SignInManager<User> signInManager, DataContext dataContext, 
            RoleManager<IdentityRole> roleManager, IEmailService emailService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _dataContext = dataContext;
            _roleManager = roleManager;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var userExist = await _userManager.FindByEmailAsync(registerDto.Email);

            if (userExist != null)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new Response { Status = "Error", Message = "User already exist!"});
            }

            var user = new User
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = true
            };

            if (await _roleManager.RoleExistsAsync(registerDto.Role))
            {
                var createdUser = await _userManager.CreateAsync(user, registerDto.Password!);

                if (!createdUser.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "Failed to create user!" });
                    
                }
                var roleResult = await _userManager.AddToRoleAsync(user, registerDto.Role);
                if (registerDto.Role == "Instructor")
                {
                    var instructor = new Instructor
                    {
                        UserId = user.Id,
                        Professional_exp = registerDto.Exp!
                    };
                    _dataContext.Instructors.Add(instructor);
                }
                else if (registerDto.Role == "Student")
                {
                    var student = new Student
                    {
                        UserId = user.Id
                    };
                    _dataContext.Students.Add(student);
                }
                await _dataContext.SaveChangesAsync();

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, Request.Scheme);
                if (confirmationLink == null)
                {
                    throw new InvalidOperationException("Unable to generate confirmation link.");
                }
                var message = new Message(new string[] { user.Email! }, "Confirmation email link", confirmationLink!);
                _emailService.SendEmail(message);

                return StatusCode(StatusCodes.Status201Created,
                    new Response { Status = "Success", Message = "User created & Email sent!" });
            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "Something went wrong!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);


            if (user!.TwoFactorEnabled)
            {
                await _signInManager.SignOutAsync();
                await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, true);

                var otp = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                var message = new Message(new string[] { user.Email! }, "OTP Confirmation", otp!);
                _emailService.SendEmail(message);
                return StatusCode(StatusCodes.Status200OK,
                    new Response { Status = "Success", Message = "We have sent an OTP to your email!" });
            }
            if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                var claims = new List<Claim>
                    {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };
                var userRole = await _userManager.GetRolesAsync(user);
                foreach (var role in userRole)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
                return StatusCode(StatusCodes.Status200OK,
                    new Response { Status = "Success", Message = "Login Successfully" });
            }
            return Unauthorized();
        }

        [HttpPost("login-2f")]
        public async Task<IActionResult> LoginWith2F(string otp, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var result = await _signInManager.TwoFactorSignInAsync("Email", otp, false, false);

            if (result.Succeeded)
            {
                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };
                    var userRole = await _userManager.GetRolesAsync(user);
                    foreach (var role in userRole)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var jwtToken = _tokenService.CreateToken(claims);

                    return Ok(new
                    {
                        token = jwtToken
                    });
                }
            }
            return Unauthorized(result.Succeeded);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest("User is not exist!");
            } 
            else
            {
                var result = await _userManager.DeleteAsync(user);
            }

            return Ok("complete");
        }

        [HttpPut("edit")]
        public async Task<IActionResult> EditUser(EditUserDto editUserDto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Deo co");
            }

            user.UserName = editUserDto.UserName;
            user.Email = editUserDto.Email;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok("Update successfully");
            }
            return BadRequest("Error");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                    return StatusCode(StatusCodes.Status200OK,
                        new Response { Status = "Success", Message = "Email sent successfully!" });
            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "Something went wrong!" });
        }
        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgotPassword(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return BadRequest("This user is not exist!");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var forgotPasswordLink = Url.Action(nameof(ResetPassword), "Account", new { token, email = user.Email }, Request.Scheme);
            var message = new Message(new string[] { user.Email! }, "Reset password email link", forgotPasswordLink);
            _emailService.SendEmail(message);

            return StatusCode(StatusCodes.Status200OK,
                new Response { Status = "Success", Message = "Password change request is sent to you email successfully"});
        }

        [HttpGet("reset-password")]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPassword
            {
                Token = token,
                Email = email
            };
            return Ok(new { model });
        }
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user != null)
            {
                var changePassword = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
                if (!changePassword.Succeeded)
                {
                    return BadRequest("Some thing went wrong!");
                }
                return StatusCode(StatusCodes.Status200OK,
                    new Response { Status = "Success", Message = "Password change request is sent to you email successfully" });
            }

            return BadRequest("This user is not exist!");
        }

    }
}
