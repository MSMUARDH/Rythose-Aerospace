using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class OrderCustomozationDetailsRepository : Repository<OrderCustomozationDetails>, IOrderCustomozationDetailsRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderCustomozationDetailsRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(OrderCustomozationDetails orderCustomozationDetails)
        {
            var obj = _db.OrderCustomozationDetails.FirstOrDefault(x => x.Id == orderCustomozationDetails.Id);
            if (obj != null)
            {
                obj = orderCustomozationDetails;
            }
            _db.Update(obj);
        }
    }
}