using Garage_Management.Base.Interface;
using Garage_Management.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Infrastructure.Repositories
{
    /// <summary>
    /// Base repository class for implementing Repository pattern
    /// </summary>
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly DbSet<T> dbSet;
        private readonly AppDbContext context;

        protected BaseRepository(AppDbContext context)
        {
            this.context = context;
            this.dbSet = this.context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await this.dbSet.FindAsync(id).ConfigureAwait(false);
        }

        public void Add(T entity)
        {
            this.dbSet.Add(entity);
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken)
        {
            await this.dbSet.AddAsync(entity, cancellationToken);
        }

        public void Update(T entity)
        {
            this.dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            this.dbSet.Remove(entity);
        }

        public IQueryable<T> GetAll()
        {
            return this.dbSet;
        }

        public async Task<int> SaveAsync(CancellationToken cancellationToken)
        {
            return await this.context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public void AddRange(IReadOnlyCollection<T> entities)
        {
            this.dbSet.AddRange(entities);
        }
    }

}
