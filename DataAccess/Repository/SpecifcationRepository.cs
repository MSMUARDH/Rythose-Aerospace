using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class SpecifcationRepository : Repository<Specifcation>, ISpecifcationRepository
    {
        private readonly ApplicationDbContext _db;

        public SpecifcationRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Specifcation specifcation)
        {
            var obj = _db.Specifcations.FirstOrDefault(x => x.Id == specifcation.Id);
            if (obj != null)
            {
                obj.Title = specifcation.Title;
                obj.Description = specifcation.Description;
            }
            _db.Update(obj);

        }
    }
}