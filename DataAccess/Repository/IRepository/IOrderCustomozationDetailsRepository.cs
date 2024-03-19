using Models;

namespace DataAccess.Repository.IRepository
{
    public interface IOrderCustomozationDetailsRepository : IRepository<OrderCustomozationDetails>
    {
        void Update(OrderCustomozationDetails orderCustomozationDetails);
    }
}