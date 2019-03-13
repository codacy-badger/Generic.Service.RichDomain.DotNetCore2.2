using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Generic.Repository.Extensions.Filter;
using Generic.Repository.Extensions.Commom;
using Generic.Repository.Models.BaseEntity.BaseFilter;
using Microsoft.EntityFrameworkCore;
using Generic.Repository.Extensions.Properties;

namespace Generic.Repository.Repository.Base
{
    ///<summary>
    /// This is a Base Actions which any entity or repository should be have.
    /// E - Entity Type, F inheritance of BaseFilter or BaseFilter
    ///</summary>
    public abstract class BaseRepository<E, F> : IBaseRepository<E, F>
    where E : class
    where F : class, IBaseFilter
    {
        private E item;
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
            _dataInclusionNameField = dataInclusionNameField ??
                throw new ArgumentNullException(nameof(dataInclusionNameField));
            _context = context;
        }

        public virtual IQueryable<E> GetAll(bool EnableAsNoTracking) => EnableAsNoTracking ? _context.Set<E>().AsNoTracking() : _context.Set<E>();

        public virtual IQueryable<E> GetAllBy(Expression<Func<E, bool>> predicate, bool EnableAsNoTracking) => predicate != null ?
        GetAll(EnableAsNoTracking).Where(predicate) : GetAll(EnableAsNoTracking);

        public virtual IQueryable<E> FilterAll(F filter, bool EnableAsNoTracking) => GetAllBy(filter.GenerateLambda<E, F>(), EnableAsNoTracking);

        public virtual async Task<E> GetByAsync(Expression<Func<E, bool>> predicate) => await _context.Set<E>().SingleOrDefaultAsync(predicate);

        public virtual async Task<E> CreateAsync()
        {
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
            SetState(EntityState.Modified, item);
            await _context.SaveChangesAsync();
        }

        public void Map(E item)
        {
            SetThis(item);
        }

        private void SetThis(E item)
        {
            this.item = ReturnE();
            Properties<E>.CacheGet[typeE.Name].ToList().ForEach(get =>
            {
                if (Properties<E>.CacheSet[typeE.Name].TryGetValue(get.Key, out Action<E, object> set))
                {
                    if (get.Key.Equals(_dataInclusionNameField))
                        set(this.item, DateTime.Now);
                    else set(this.item, get.Value(item));
                }
            });
        }

        private E ReturnE() => (E)Convert.ChangeType(this, typeE);

        private void SetState(EntityState state, E item) => _context.Entry<E>(item).State = state;
    }
}