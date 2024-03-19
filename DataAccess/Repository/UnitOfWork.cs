using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            ApplicationRole = new ApplicationRoleRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            //Books = new BooksRepository(_db);
            Order = new OrderRepository(_db);
            OrderDetails = new OrderDetailsRepository(_db);
            Cart = new CartRepository(_db);
            CartDetails = new CartDetailsRepository(_db);
            //Comments = new CommentsRepository(_db);
            SP_Call = new SP_Call(_db);

            aircraft = new AircraftRepository(_db);
            aircrafImages = new AircrafImagesRepository(_db);
            availableCustomozation = new AvailableCustomozationRepository(_db);
            specifcation = new SpecifcationRepository(_db);
            CartCustomozationDetails = new CartCustomozationDetailsRepository(_db);
            shippingDetail = new ShippingDetailRepository(_db);
            payment = new PaymentRepository(_db);
            orderCustomozationDetails = new OrderCustomozationDetailsRepository(_db);
            team= new TeamRepository(_db);
            job= new JobRepository(_db);
            city = new CityRepository(_db);
            shippingSet = new ShippingSetRepository(_db);
        }

        public IApplicationRoleRepository ApplicationRole { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        //public IBooksRepository Books { get; private set; }
        public IOrderRepository Order { get; private set; }
        public IOrderDetailsRepository OrderDetails { get; private set; }
        public ICartRepository Cart { get; private set; }
        public ICartDetailsRepository CartDetails { get; private set; }
        //public ICommentsRepository Comments { get; private set; }
        public ISP_Call SP_Call { get; private set; }

        public IAircraftRepository aircraft { get; private set; }
        public IAircrafImagesRepository aircrafImages { get; private set; }
        public IAvailableCustomozationRepository availableCustomozation { get; private set; }
        public ISpecifcationRepository specifcation { get; private set; }
        public ICartCustomozationDetailsRepository CartCustomozationDetails { get; private set; }

        public IShippingDetailRepository shippingDetail { get; private set; }
        public IPaymentRepository payment { get; private set; }

        public IOrderCustomozationDetailsRepository orderCustomozationDetails { get; private set; }

        public ITeamRepository team { get; private set; }
        public IJobRepository job { get; private set; }

        public ICityRepository city { get; private set; }
        public IShippingSetRepository shippingSet { get; private set; }



        public void Dispose()
        {
            _db.Dispose();
        }

        public void SaveAsync()
        {
            _db.SaveChangesAsync().GetAwaiter().GetResult();
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _db.Database.BeginTransactionAsync();
        }
        public async void CommitAsync(IDbContextTransaction transaction)
        {
            await transaction.CommitAsync();
        }
        public async void RollbackAsync(IDbContextTransaction transaction)
        {
            await transaction.RollbackAsync();
        }
    }
}
