using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Generic.Repository.Entity.IFilter;
using Generic.Repository.Extension.Filter;
using Microsoft.EntityFrameworkCore;

namespace Generic.Repository.Base
{
    ///<summary>
    /// This is a Base Actions which any entity or repository should be have.
    /// E - Entity Type, F inheritance of BaseFilter or BaseFilter
    ///</summary>
    public abstract class BaseRepository<E, F> : IBaseRepository<E, F>
        where E : class
    where F : IBaseFilter
    {
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

        public virtual IQueryable<E> GetAll() => _context.Set<E>().AsNoTracking();

        public virtual IQueryable<E> GetAllBy(Expression<Func<E, bool>> predicate) => predicate != null ? GetAll().Where(predicate) : GetAll();

        public virtual IQueryable<E> FilterAll(F filter) => GetAllBy(filter.GenerateLambda<E, F>());

        public virtual async Task<E> GetByIdAsync(long id) => await _context.Set<E>().FindAsync(id);

        public virtual async Task<E> GetByAsync(Expression<Func<E, bool>> predicate) => await _context.Set<E>().FirstOrDefaultAsync(predicate);

        public virtual async Task<E> CreateAsync()
        {
            SetDateInclusion();
            var item = ReturnE();
            SetState(EntityState.Added, item);
            await _context.SaveChangesAsync();
            return item;
        }

        public virtual async Task DeleteAsync(long id)
        {
            _context.Remove(await GetByIdAsync(id));
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
            foreach (var prop in this.GetType().GetProperties())
            {
                prop.SetValue(this, item.GetType().GetProperty(prop.Name).GetValue(item) ?? null);
            }
        }

        private void SetDateInclusion()
        {
            if (this.GetType().GetProperties().FirstOrDefault(x => x.Name.Equals(_dataInclusionNameField)) != null)
                this.GetType().GetProperty(_dataInclusionNameField).SetValue(this, DateTime.Now);
        }

        private E ReturnE() => (E)Convert.ChangeType(this, typeof(E));

        private void SetState(EntityState state, E item) => _context.Entry<E>(item).State = state;
    }
}