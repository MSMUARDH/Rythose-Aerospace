using Models;

namespace DataAccess.Repository.IRepository
{
    public interface ICartCustomozationDetailsRepository : IRepository<CartCustomozationDetails>
    {
        void Update(CartCustomozationDetails cartCustomozationDetails);
    }
}