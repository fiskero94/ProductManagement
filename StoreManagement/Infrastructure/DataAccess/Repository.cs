using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess
{
    public abstract class Repository<T>
    {
        public Repository(DbContext context)
        {
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
        }

        public abstract Task<IEnumerable<T>> GetAllAsync();
        public abstract Task<T> GetAsync(int id);
        public abstract Task CreateAsync(T entity);
        public abstract Task UpdateAsync(T old, T updated);
        public abstract Task DeleteAsync(T entity);
    }
}