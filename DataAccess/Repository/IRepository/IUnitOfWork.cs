using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IApplicationRoleRepository ApplicationRole { get; }
        IApplicationUserRepository ApplicationUser { get; }
        //IBooksRepository Books { get; }
        ICartRepository Cart { get; }
        ICartDetailsRepository CartDetails { get; }
        IOrderRepository Order { get; }
        IOrderDetailsRepository OrderDetails { get; }
        //ICommentsRepository Comments { get; }
        ISP_Call SP_Call { get; }

        IAircraftRepository aircraft { get; }
        IAircrafImagesRepository aircrafImages { get; }
        IAvailableCustomozationRepository availableCustomozation { get; }
        ISpecifcationRepository specifcation { get; }
        ICartCustomozationDetailsRepository CartCustomozationDetails { get; }

        IShippingDetailRepository shippingDetail { get; }
        IPaymentRepository payment { get; }
        IOrderCustomozationDetailsRepository orderCustomozationDetails { get; }
        ITeamRepository team { get; }

        IJobRepository job { get; }
        ICityRepository city { get; }
        IShippingSetRepository shippingSet { get; }

        void SaveAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        void CommitAsync(IDbContextTransaction transaction);
        void RollbackAsync(IDbContextTransaction transaction);
    }
}
