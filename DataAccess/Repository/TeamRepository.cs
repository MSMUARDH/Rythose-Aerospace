using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class TeamRepository : Repository<Team>, ITeamRepository
    {
        private readonly ApplicationDbContext _db;

        public TeamRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Team team)
        {
            var obj = _db.Team.FirstOrDefault(x => x.Id == team.Id);
            if (obj != null)
            {
                obj.Code = team.Code;
                obj.Name = team.Name;
                obj.NoOfEmployees = team.NoOfEmployees;
                obj.CurStatus = team.CurStatus;
               
            }
            _db.Update(obj);
        }
    }
}