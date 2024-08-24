using Coursea.Data;
using Coursea.Interfaces;
using Coursea.Models;
using Microsoft.EntityFrameworkCore;

namespace coursea.data.repositories
{
    public class CourseRepository : ICourseRepo
    {
        public readonly DataContext _datacontext;
        public CourseRepository(DataContext datacontext)
        {
            _datacontext = datacontext;
        }
        public void AddCourse(Course course)
        {
            _datacontext.Courses.Add(course);
        }

        public async Task<IEnumerable<Course>> GetAll()
        {
            return await _datacontext.Courses.ToListAsync();
        }

        public async Task<Course> GetById(int id)
        {
            return await _datacontext.Courses.FirstOrDefaultAsync(c => c.Id == id);
        }

        public void RemoveCourse(int id)
        {
            var getcourse = _datacontext.Courses.FirstOrDefault(c => c.Id == id);
            _datacontext.Courses.Remove(getcourse);
        }
    }
}
