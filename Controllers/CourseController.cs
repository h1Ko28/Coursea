using AutoMapper;
using Coursea.Data;
using Coursea.Dto.Course;
using Coursea.Interfaces;
using Coursea.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Coursea.Controllers
{

    public class CourseController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public CourseController(IUnitOfWork unitOfWork, UserManager<User> userManager,
                                DataContext dataContext, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _dataContext = dataContext;
            _mapper = mapper;
        }

        [HttpGet("getallcourse")]
        public async Task<IActionResult> GetAll()
        {
            var allCourse = await _unitOfWork.CourseRepo.GetAll();
            var coursesDto = _mapper.Map<IEnumerable<ListCourseDto>>(allCourse);
            return Ok(coursesDto);
        }

        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var course = await _unitOfWork.CourseRepo.GetById(id);
            return Ok(course);
        }

        [HttpPost("addcourse")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> AddCourse(AddCourseDto addCourseDto)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return BadRequest("User not exist");

            var instructor = _dataContext.Instructors.FirstOrDefault(i => i.UserId == user.Id);

            if (instructor == null)
                return BadRequest("you are not instructor");

            var newCourse = new Course
            {
                Title = addCourseDto.Title,
                Description = addCourseDto.Description,
                Fee = addCourseDto.Fee,
                Code_course = GenerateCourseCode(),
                InstructorId = instructor.Id,
                InstructorName = user.UserName,
                CategoryId = addCourseDto.CategoryId
            };

            _unitOfWork.CourseRepo.AddCourse(newCourse);
            await _unitOfWork.SaveAsync();

            return StatusCode(201);
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> RemoveCourse(int id)
        {
            _unitOfWork.CourseRepo.RemoveCourse(id);
            await _unitOfWork.SaveAsync();
            return StatusCode(200);
        }

        private string GenerateCourseCode()
        {
            string year = DateTime.Now.Year.ToString().Substring(2, 2);
            int randomDigits = new Random().Next(100000, 999999);
            return $"ST{year}{randomDigits}";
        }
    }
}
