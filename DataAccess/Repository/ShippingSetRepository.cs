using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class ShippingSetRepository : Repository<ShippingSet>, IShippingSetRepository
    {
        private readonly ApplicationDbContext _db;

        public ShippingSetRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(ShippingSet shippingSet)
        {
            var obj = _db.ShippingSet.FirstOrDefault(x => x.ShippingSetId == shippingSet.ShippingSetId);
            if (obj != null)
            {
                obj.CityId = shippingSet.CityId;
                obj.Amount = shippingSet.Amount;
               
            }
            _db.Update(obj);
        }
    }
}