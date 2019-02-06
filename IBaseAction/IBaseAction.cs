using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GenericModel.Filter;

namespace GenericModel.Action
{
    public interface IBaseAction<E, F>
    where F : BaseFilter
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
        IQueryable<E> GetAll();
        ///<summary>
        /// Return all data filtred
        ///</summary>
        ///<param name="filter">Filter to apply</param>
        IQueryable<E> FilterAll(F filter);
        ///<summary>
        /// Return all data with pass on the predicate
        ///</summary>
        ///<param name="predicate">Condition to apply on data</param>
        IQueryable<E> GetAllBy(Expression<Func<E, bool>> predicate);
        Task<E> GetByIdAsync(long id);
        Task<E> GetByAsync(Expression<Func<E, bool>> predicate);
        Task<E> CreateAsync();
        Task UpdateAsync();
        Task DeleteAsync(long id);
    }
}