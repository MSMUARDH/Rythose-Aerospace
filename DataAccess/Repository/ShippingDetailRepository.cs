using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class ShippingDetailRepository : Repository<ShippingDetail>, IShippingDetailRepository
    {
        private readonly ApplicationDbContext _db;

        public ShippingDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(ShippingDetail shippingDetail)
        {
            var obj = _db.ShippingDetail.FirstOrDefault(x => x.ShippingID == shippingDetail.ShippingID);
            if (obj != null)
            {
                obj.FirstName = shippingDetail.FirstName;
                obj.LastName = shippingDetail.LastName;
                obj.Email = shippingDetail.Email;
                obj.Mobile = shippingDetail.Mobile;
                obj.Address = shippingDetail.Address;
                obj.CityId = shippingDetail.CityId;
                obj.PostCode = shippingDetail.PostCode;
                obj.Country = shippingDetail.Country;
            }
            _db.Update(obj);
        }
    }
}