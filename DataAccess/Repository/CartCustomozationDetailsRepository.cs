using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class CartCustomozationDetailsRepository : Repository<CartCustomozationDetails>, ICartCustomozationDetailsRepository
    {
        private readonly ApplicationDbContext _db;

        public CartCustomozationDetailsRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(CartCustomozationDetails cartCustomozationDetails)
        {
            var obj = _db.CartCustomozationDetails.FirstOrDefault(x => x.Id == cartCustomozationDetails.Id);
            if (obj != null)
            {
                obj = cartCustomozationDetails;
            }
        }
    }
}