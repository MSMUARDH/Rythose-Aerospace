using Models;

namespace DataAccess.Repository.IRepository
{
    public interface IShippingDetailRepository : IRepository<ShippingDetail>
    {
        void Update(ShippingDetail shippingDetail);
    }
}