﻿using Coursea.Data;
using Coursea.Dto.Account;
using Coursea.Interfaces;
using Coursea.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EmailManagement.Services;
using EmailManagement.Models;

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
                SecurityStamp = Guid.NewGuid().ToString()
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
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            var password = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (user != null && password)
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
                return Ok(jwtToken);
            }
            return Unauthorized();
        }

        [HttpGet("confirmemail")]
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
        public IActionResult testMail()
        {
            var message = new Message(new string[] { "mautraunhatxom28@gmail.com" }, "test", "yolo");
            _emailService.SendEmail(message);
            return Ok("successful");
        }
    }
}
