using Coursea.Models;

namespace Coursea.Interfaces
{
    public interface ICourseRepo
    {
        void AddCourse(Course course);
        void RemoveCourse(int id);
        Task<IEnumerable<Course>> GetAll();
        Task<Course> GetById(int id);
    }
}
