using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class AircraftRepository : Repository<Aircraft>, IAircraftRepository
    {
        private readonly ApplicationDbContext _db;

        public AircraftRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Aircraft aircraft)
        {
            var obj = _db.Aircraft.FirstOrDefault(x => x.AircratfId == aircraft.AircratfId);
            if (obj != null)
            {
                obj.Code = aircraft.Code;
                obj.Name = aircraft.Name;
                obj.Model = aircraft.Model;
                obj.Category = aircraft.Category;
                obj.Description = aircraft.Description;
                obj.ImgUrl = aircraft.ImgUrl;
                obj.Price = aircraft.Price;
                obj.Qty = aircraft.Qty;
                obj.CurStatus = aircraft.CurStatus;
            }
            _db.Update(obj);
        }
    }
}