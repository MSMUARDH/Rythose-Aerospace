using Models;

namespace DataAccess.Repository.IRepository
{
    public interface ITeamRepository : IRepository<Team>
    {
        void Update(Team team);
    }
}