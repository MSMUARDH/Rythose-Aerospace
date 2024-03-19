using Models;

namespace DataAccess.Repository.IRepository
{
    public interface ISpecifcationRepository : IRepository<Specifcation>
    {
        void Update(Specifcation specifcation);
    }
}