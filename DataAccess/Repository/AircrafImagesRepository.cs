using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class AircrafImagesRepository : Repository<AircrafImages>, IAircrafImagesRepository
    {
        private readonly ApplicationDbContext _db;

        public AircrafImagesRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(AircrafImages aircrafImages)
        {
            var obj = _db.AircrafImages.FirstOrDefault(x => x.Id == aircrafImages.Id);
            if (obj != null)
            {
                obj.ImgUrl =aircrafImages.ImgUrl ;
            }
        }
    }
}