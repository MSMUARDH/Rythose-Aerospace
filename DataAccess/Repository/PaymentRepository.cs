using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        private readonly ApplicationDbContext _db;

        public PaymentRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Payment payment)
        {
            var obj = _db.Payment.FirstOrDefault(x => x.PaymentID == payment.PaymentID);
            if (obj != null)
            {
                obj=payment;
            }
        }
    }
}