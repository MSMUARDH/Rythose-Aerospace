using Models;

namespace DataAccess.Repository.IRepository
{
    public interface IAircrafImagesRepository : IRepository<AircrafImages>
    {
        void Update(AircrafImages aircrafImages);
    }
}