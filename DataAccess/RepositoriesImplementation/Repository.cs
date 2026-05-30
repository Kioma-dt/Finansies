using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccess.RepositoriesImplementation
{
    public class Repository<T>
        : IRepository<T>
        where T : UsersEntity
    {
        protected readonly IDbContextFactory<FinansiesDbContext> _dbContextFactory;

        public Repository(IDbContextFactory<FinansiesDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task Add(T entity)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            await dbContext.Set<T>().AddAsync(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task Delete(T entity)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var exists = await dbContext.Set<T>()
                .AnyAsync(x => x.Id == entity.Id);

            if (!exists)
                return;

            dbContext.Set<T>().Attach(entity);
            dbContext.Set<T>().Remove(entity);

            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAll(Guid userId, 
            params Expression<Func<T, object>>[] includes)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            IQueryable<T> query = dbContext.Set<T>();

            query = query.Where(x => x.UserId == userId);

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public async Task<T?> GetById(Guid userId, Guid id, params Expression<Func<T, object>>[] includes)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            IQueryable<T> query = dbContext.Set<T>();

            query = query.Where(x => x.UserId == userId);

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Update(T entity)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            if (entity is null)
            {
                throw new ArgumentNullException();
            }

            var exists = await dbContext.Set<T>()
                .AnyAsync(x => x.Id == entity.Id);

            if (!exists)
                return;

            dbContext.Entry(entity).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }
    }
}
