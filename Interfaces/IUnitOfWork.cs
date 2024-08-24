namespace Coursea.Interfaces
{
    public interface IUnitOfWork
    {
        ICategoryRepo CategoryRepo { get; }
        ICourseRepo CourseRepo { get; }
        Task<bool> SaveAsync();
    }
}
