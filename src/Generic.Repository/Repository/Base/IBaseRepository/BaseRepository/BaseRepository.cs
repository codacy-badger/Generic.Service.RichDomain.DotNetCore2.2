using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Generic.Repository.Extensions.Filter;
using Generic.Repository.Extensions.Commom;
using Generic.Repository.Models.BaseEntity.BaseFilter;
using Microsoft.EntityFrameworkCore;

namespace Generic.Repository.Repository.Base.IBaseRepository
{
    ///<summary>
    /// This is a Base Actions which any entity or repository should be have.
    /// E - Entity Type, F inheritance of BaseFilter or BaseFilter
    ///</summary>
    public abstract class BaseRepository<E, F> : IBaseRepository<E, F>
    where E : class
    where F : IBaseFilter
    {
        private static readonly Type typeE = typeof(E);
        protected readonly DbContext _context;
        private readonly string _dataInclusionNameField;
        public BaseRepository(DbContext context)
        {
            _context = context;
            _dataInclusionNameField = "dateInclusion";
        }

        public BaseRepository(DbContext context, string dataInclusionNameField)
        {
            _context = context;
            _dataInclusionNameField = dataInclusionNameField ??
                throw new ArgumentNullException(nameof(dataInclusionNameField));
        }

        public virtual IQueryable<E> GetAll(bool AsNoTrackingDefault) => AsNoTrackingDefault ? _context.Set<E>().AsNoTracking() : _context.Set<E>();

        public virtual IQueryable<E> GetAllBy(Expression<Func<E, bool>> predicate, bool AsNoTrackingDefault) => predicate != null ?
        GetAll(AsNoTrackingDefault).Where(predicate) : GetAll(AsNoTrackingDefault);

        public virtual IQueryable<E> FilterAll(F filter, bool AsNoTrackingDefault) => GetAllBy(filter.GenerateLambda<E, F>(), AsNoTrackingDefault);

        public virtual async Task<E> GetByAsync(Expression<Func<E, bool>> predicate) => await _context.Set<E>().SingleOrDefaultAsync(predicate);

        public virtual async Task<E> CreateAsync()
        {
            SetDateInclusion();
            var item = ReturnE();
            SetState(EntityState.Added, item);
            await _context.SaveChangesAsync();
            return item;
        }

        public virtual async Task DeleteAsync(E entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync()
        {
            var item = ReturnE();
            SetState(EntityState.Modified, item);
            await _context.SaveChangesAsync();
        }

        public void Map(E item)
        {
            Commom.SaveOnCacheIfNonExists(typeE);
            SetThis(item);
        }

        private void SetThis(E item)
        {
            if (Commom.cache.TryGetValue(typeE, out Dictionary<string, PropertyInfo> dicProperties))
                dicProperties.Values.ToList().ForEach(x =>
                {
                    x.SetValue(this, x.GetValue(item, null));
                });
        }

        private void SetDateInclusion()
        {
            if (Commom.cache.TryGetValue(typeE, out Dictionary<string, PropertyInfo> dicProperties))
            {
                if (dicProperties.TryGetValue(_dataInclusionNameField, out PropertyInfo prop))
                {
                    prop.SetValue(this, DateTime.Now);
                }
            }
        }

        private E ReturnE() => (E)Convert.ChangeType(this, typeE);

        private void SetState(EntityState state, E item) => _context.Entry<E>(item).State = state;
    }
}