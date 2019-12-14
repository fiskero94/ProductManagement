using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.DataAccess
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetAsync(int id);
        Task CreateAsync(T entity);
        Task UpdateAsync(T old, T updated);
        Task DeleteAsync(T entity);
    }
}