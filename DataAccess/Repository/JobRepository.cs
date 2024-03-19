using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class JobRepository : Repository<Job>, IJobRepository
    {
        private readonly ApplicationDbContext _db;

        public JobRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Job job)
        {
            var obj = _db.Job.FirstOrDefault(x => x.Id == job.Id);
            if (obj != null)
            {
                obj.OrderId = job.OrderId;
                obj.TeamId = job.TeamId;
                obj.CurStatus = job.CurStatus;
            }
            _db.Update(obj);
        }
    }
}