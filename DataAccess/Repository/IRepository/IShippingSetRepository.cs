using Models;

namespace DataAccess.Repository.IRepository
{
    public interface IShippingSetRepository : IRepository<ShippingSet>
    {
        void Update(ShippingSet shippingSet);
    }
}