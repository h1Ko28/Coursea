using Coursea.Models;

namespace Coursea.Interfaces
{
    public interface IReportRepo
    {
        void Add(Report report);
        Task<List<Report>> GetAll();
        Task<Report> GetById(int id);
        void Remove(int id);
    }
}
