using Models;

namespace DataAccess.Repository.IRepository
{
    public interface IAircraftRepository : IRepository<Aircraft>
    {
        void Update(Aircraft aircraft);
    }
}