using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Generic.Service.Models.BaseModel.BaseFilter;

namespace Generic.Service.Service.Base
{
    public interface IBaseService<TValue, TFilter>
    where TFilter : IFilter
    where TValue : class
    {

        void Map(TValue item);
        
#region Query
        ///<summary>
        /// Return all data
        ///</summary>
        IQueryable<TValue> GetAll(bool EnableAsNoTracking);
        ///<summary>
        /// Return all data filtred
        ///</summary>
        ///<param name="filter">Filter to apply</param>
        IQueryable<TValue> FilterAll(TFilter filter, bool EnableAsNoTracking);
        ///<summary>
        /// Return all data with pass on the predicate
        ///</summary>
        ///<param name="predicate">Condition to apply on data</param>
        IQueryable<TValue> GetAllBy(Expression<Func<TValue, bool>> predicate, bool EnableAsNoTracking);
        /// <summary>
        /// Return first data from a informed predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<TValue> GetByAsync(Expression<Func<TValue, bool>> predicate, bool EnableAsNoTracking);
#endregion

#region COMMAND - (CREAT, UPDATE, DELETE) Without CancellationToken
        /// <summary>
        /// Save data async
        /// </summary>
        /// <returns></returns>
        Task<TValue> CreateAsync();  
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
        Task DeleteAsync();
#endregion

#region COMMAND - (CREAT, UPDATE, DELETE) With CancellationToken
        /// <summary>
        /// Save data async
        /// </summary>
        /// <returns></returns>
        Task<TValue> CreateAsync(CancellationToken token);
        /// <summary>
        /// Update data async
        /// </summary>
        /// <returns></returns>
        Task UpdateAsync(CancellationToken token);
        /// <summary>
        /// Delete data async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(CancellationToken token);
#endregion

#region COMMIT - (SAVECHANGES)
        /// <summary>
        /// Commit async transaction if useCommit is true 
        /// </summary>
        /// <returns></returns>
        Task CommitAsync();
        /// <summary>
        /// Commit async transaction if useCommit is true 
        /// </summary>
        /// <returns></returns>
        Task CommitAsync(CancellationToken cancellationToken);
#endregion  
    }
}