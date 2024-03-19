using Models;

namespace DataAccess.Repository.IRepository
{
    public interface IAvailableCustomozationRepository : IRepository<AvailableCustomozation>
    {
        void Update(AvailableCustomozation availableCustomozation);
    }
}