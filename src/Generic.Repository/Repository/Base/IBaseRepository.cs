using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Generic.Repository.Models.BaseEntity.BaseFilter;

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
        IQueryable<E> GetAll(bool AsNoTrackingDefault);
        ///<summary>
        /// Return all data filtred
        /// This method generate a lambda on runtime where:
        /// if the attribute on filter is string contains has used to compare;
        /// if the attribute on filter is long and more than 0 or do not corresponds to string type, equals has used to compare;
        /// <remarks>
        /// EX: 
        /// JSON Filter
        /// {"id": 1, "Name": "TestName"}
        /// The lambda generated corresponds to something like this: x => x.id == 1 || x.Contains("TestName")
        /// </remarks>
        ///</summary>
        ///<param name="filter">Filter to apply</param>
        IQueryable<E> FilterAll(F filter, bool AsNoTrackingDefault);
        ///<summary>
        /// Return all data with pass on the predicate
        ///</summary>
        ///<param name="predicate">Condition to apply on data</param>
        IQueryable<E> GetAllBy(Expression<Func<E, bool>> predicate, bool AsNoTrackingDefault);
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
        /// Delete dat async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(E entity);
    }
}