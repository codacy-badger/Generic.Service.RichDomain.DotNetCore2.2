using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Generic.Repository.Models.BaseModel.BaseFilter;

namespace Generic.Repository.Repository.Base
{
    public interface IBaseRepository<E, F>
    where F : IBaseFilter
    where E : class
    {
        ///<summary>
        /// Map the sended entity to this entity
        ///</summary>
        ///<param name="item">Item to map</param>
        void Map(E item);
        ///<summary>
        /// Return all data
        ///</summary>
        IQueryable<E> GetAll(bool EnableAsNoTracking);
        ///<summary>
        /// Return all data filtred
        ///</summary>
        ///<param name="filter">Filter to apply</param>
        IQueryable<E> FilterAll(F filter, bool EnableAsNoTracking);
        ///<summary>
        /// Return all data with pass on the predicate
        ///</summary>
        ///<param name="predicate">Condition to apply on data</param>
        IQueryable<E> GetAllBy(Expression<Func<E, bool>> predicate, bool EnableAsNoTracking);
        /// <summary>
        /// Return first data from a informed predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<E> GetByAsync(Expression<Func<E, bool>> predicate);
        /// <summary>
        /// Save data async
        /// </summary>
        /// <returns></returns>
        Task<E> CreateAsync();
        /// <summary>
        /// Update data async
        /// </summary>
        /// <returns></returns>
        Task UpdateAsync();
        /// <summary>
        /// Delete data async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(E entity);
        /// <summary>
        /// Commit async transaction if useCommit is true 
        /// </summary>
        /// <returns></returns>
        Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}