using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class AvailableCustomozationRepository : Repository<AvailableCustomozation>, IAvailableCustomozationRepository
    {
        private readonly ApplicationDbContext _db;

        public AvailableCustomozationRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(AvailableCustomozation availableCustomozation)
        {
            var obj = _db.AvailableCustomozations.FirstOrDefault(x => x.Id == availableCustomozation.Id);
            if (obj != null)
            {
                obj.CustomzationType = availableCustomozation.CustomzationType;
                obj.CustomzationValue = availableCustomozation.CustomzationValue;
                obj.Price = availableCustomozation.Price;
            }
            _db.Update(obj);
        }
    }
}