using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GenericModel.Filter;
using Microsoft.EntityFrameworkCore;

namespace GenericModel.Action
{
    ///<summary>
    /// This is a Base Actions which any entity or repository should be have.
    /// C - DbContext if you need use,  E - Entity Type, F inheritance of BaseFilter or BaseFilter
    ///</summary>
    public abstract class BaseAction<E, F, C> : IBaseAction<E, F>
        where E : class
        where F : BaseFilter
        where C : DbContext
    {
        protected readonly C _context;

        public BaseAction(C context) => _context = context;
        public virtual IQueryable<E> GetAll() => _context.Set<E>().AsNoTracking();

        public virtual IQueryable<E> GetAllBy(Expression<Func<E, bool>> predicate) => GetAll().Where(predicate);

        public virtual async Task<E> GetByIdAsync(long id) => await _context.Set<E>().FindAsync(id);

        public virtual async Task<E> GetByAsync(Expression<Func<E, bool>> predicate) => await _context.Set<E>().FirstOrDefaultAsync(predicate);

        public virtual async Task<E> CreateAsync()
        {
            var item = ReturnE();
            SetState(EntityState.Added, item);
            await _context.SaveChangesAsync();
            return item;
        }

        private E ReturnE() => (E)Convert.ChangeType(this, typeof(E));
        private void SetState(EntityState state, E item) => _context.Entry<E>(item).State = state;
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

        public abstract IQueryable<E> FilterAll(F filter);
    }
}