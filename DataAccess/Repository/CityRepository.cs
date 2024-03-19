using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class CityRepository : Repository<City>, ICityRepository
    {
        private readonly ApplicationDbContext _db;

        public CityRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(City city)
        {
            var obj = _db.City.FirstOrDefault(x => x.Id == city.Id);
            if (obj != null)
            {
                obj.Name = city.Name;
            }
            _db.Update(obj);
        }
    }
}