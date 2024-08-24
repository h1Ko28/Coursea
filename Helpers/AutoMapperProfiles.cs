using AutoMapper;
using Coursea.Dto.Account;
using Coursea.Dto.Course;
using Coursea.Models;

namespace Coursea.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles() 
        {
            CreateMap<Course, ListCourseDto>().ReverseMap();
            CreateMap<User, EditUserDto>().ReverseMap();
        }
    }
}
