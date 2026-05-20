using BuisnessLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic.Repositories
{
    public interface IRepository<T>
        where T : UsersEntity
    {
        Task<IEnumerable<T>> GetAll(Guid userId, 
            params Expression<Func<T, object>>[] includes);
        Task<T?> GetById(Guid userId, 
            Guid id,
            params Expression<Func<T, object>>[] includes);
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
    }
}
