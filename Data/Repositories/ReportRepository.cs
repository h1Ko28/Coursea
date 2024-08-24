using Coursea.Interfaces;
using Coursea.Models;
using Microsoft.EntityFrameworkCore;

namespace Coursea.Data.Repositories
{
    public class ReportRepository : IReportRepo
    {
        private readonly DataContext _dataContext;
        public ReportRepository(DataContext dataContext) 
        {
            _dataContext = dataContext;
        }
        public void Add(Report report)
        {
            _dataContext.Add(report);
        }

        public async Task<List<Report>> GetAll()
        {
            return await _dataContext.Reports.ToListAsync(); ;
        }

        public async Task<Report> GetById(int id)
        {
            return await _dataContext.Reports.FindAsync(id);
        }

        public void Remove(int id)
        {
            var report = _dataContext.Reports.Find(id);
            _dataContext.Reports.Remove(report);
        }
    }
}
