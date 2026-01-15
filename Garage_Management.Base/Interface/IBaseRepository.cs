using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Base.Interface
{
    /// <summary>
    /// Interface for Repository pattern
    /// </summary>
    public interface IBaseRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);

        void Add(T entity);

        Task AddAsync(T entity, CancellationToken cancellationToken);

        void Update(T entity);

        void Delete(T entity);

        IQueryable<T> GetAll();

        Task<int> SaveAsync(CancellationToken cancellationToken);

        void AddRange(IReadOnlyCollection<T> entities);
    }
}
