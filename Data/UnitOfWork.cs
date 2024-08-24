using coursea.data.repositories;
using Coursea.Data.Repositories;
using Coursea.Interfaces;

namespace Coursea.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _dataContext;
        public UnitOfWork(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public ICategoryRepo CategoryRepo => new CategoryRepository(_dataContext);
        public ICourseRepo CourseRepo => new CourseRepository(_dataContext);
        public IReportRepo ReportRepo => new ReportRepository(_dataContext);

        public async Task<bool> SaveAsync()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }
    }
}
