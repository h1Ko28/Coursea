using AutoMapper;
using Coursea.Data;
using Coursea.Dto.Account;
using Coursea.Interfaces;
using Coursea.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Coursea.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;
        private readonly DataContext _dataContext;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<User> userManager, ITokenService tokenService, 
            SignInManager<User> signInManager, DataContext dataContext, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _dataContext = dataContext;
            _roleManager = roleManager;
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
                var createdUser = await _userManager.CreateAsync(user, registerDto.Password);

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
                        Professional_exp = registerDto.Exp
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
            }
            return StatusCode(StatusCodes.Status201Created,
                    new Response { Status = "Success", Message = "User created" });
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
    }
}
