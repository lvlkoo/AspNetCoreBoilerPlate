using System.Linq;
using System.Threading.Tasks;

namespace Boilerplate.Repository
{
    public interface IRepository<T> where T: class
    {
        IQueryable<T> Entities { get; }
        Task<T> Find(object id);
        Task Add(T entity);
        Task Update(T entity, T updated);
        Task Remove(T entity);
    }
}
