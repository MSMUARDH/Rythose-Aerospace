using Models;

namespace DataAccess.Repository.IRepository
{
    public interface IJobRepository : IRepository<Job>
    {
        void Update(Job job);
    }
}