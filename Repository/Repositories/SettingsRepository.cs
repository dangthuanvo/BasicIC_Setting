using Repository.EF;
using Repository.Interfaces;

namespace Repository.Repositories
{
    public class SettingsRepository<T> : BaseRepositorySql<T>, IRepositorySql<T> where T : class
    {
        public SettingsRepository() : base(new M02_BasicEntities())
        {
        }
    }
}
