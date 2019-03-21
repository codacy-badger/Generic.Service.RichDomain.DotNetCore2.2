using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Generic.Service.Extensions.Commom;
using Generic.Service.Extensions.Filter;
using Generic.Service.Models.BaseModel.BaseFilter;
using Microsoft.EntityFrameworkCore;

namespace Generic.Service.Service.Base {
    ///<summary>
    /// This is a Base Service implementation which any entity or Service should be have.
    /// TValue - Entity Type, F inheritance of IBaseFilter or IBaseFilter
    ///</summary>
    public abstract class BaseService<TValue, TFilter> : IBaseService<TValue, TFilter>
        where TValue : class
    where TFilter : class, IBaseFilter {
        private TValue item;
        protected readonly DbContext _context;
        private readonly string _includeDateNameField;
        private readonly bool _useCommit;

        public BaseService (DbContext context) {
            _context = context;
            _useCommit = false;
            _includeDateNameField = "dateInclusion";

        }

        public BaseService (DbContext context, bool useCommit) {
            _includeDateNameField = "dateInclusion";
            _useCommit = useCommit;
            _context = context;
        }

        public BaseService (DbContext context, string includeDateNameField) {
            _includeDateNameField = includeDateNameField ??
                throw new ArgumentNullException (nameof (includeDateNameField));
            _useCommit = false;
            _context = context;
        }

        public BaseService (DbContext context, string includeDateNameField, bool useCommit) {
            _includeDateNameField = includeDateNameField ??
                throw new ArgumentNullException (nameof (includeDateNameField));
            _useCommit = useCommit;
            _context = context;
        }

        public virtual IQueryable<TValue> GetAll (bool EnableAsNoTracking) => EnableAsNoTracking ? _context.Set<TValue> ().AsNoTracking () : _context.Set<TValue> ();

        public virtual IQueryable<TValue> GetAllBy (Expression<Func<TValue, bool>> predicate, bool EnableAsNoTracking) => predicate != null ?
            GetAll (EnableAsNoTracking).Where (predicate) : GetAll (EnableAsNoTracking);

        public virtual IQueryable<TValue> FilterAll (TFilter filter, bool EnableAsNoTracking) => GetAllBy (filter.GenerateLambda<TValue, TFilter> (), EnableAsNoTracking);

        public virtual async Task<TValue> GetByAsync (Expression<Func<TValue, bool>> predicate) => await _context.Set<TValue> ().SingleOrDefaultAsync (predicate);

        public virtual async Task<TValue> CreateAsync () {
            SetState (EntityState.Added, this.item);
            if (!_useCommit)
                await CommitAsync ();
            return this.item;
        }

        public virtual async Task UpdateAsync () {
            SetState (EntityState.Modified, this.item);
            if (!_useCommit)
                await CommitAsync ();
        }

        public virtual async Task DeleteAsync (TValue entity) {
            _context.Remove (entity);
            if (!_useCommit)
                await CommitAsync ();
        }

        public void Map (TValue item) {
            SetThis (item);
        }

        private void SetThis (TValue item) {
            this.item = ReturnE ();
            Commom.CacheGet[typeof (TValue).Name].ToList ().ForEach (get => {
                if (Commom.CacheSet[typeof (TValue).Name].TryGetValue (get.Key, out Action<object, object> set)) {
                    if (get.Key.Equals (_includeDateNameField))
                        set (this.item, DateTime.Now);
                    else set (this.item, get.Value (item));
                }
            });
        }

        public Task CommitAsync (CancellationToken cancellationToken = default (CancellationToken)) => _context.SaveChangesAsync (cancellationToken);

        private TValue ReturnE () => (TValue) Convert.ChangeType (this, typeof (TValue));

        private void SetState (EntityState state, TValue item) => _context.Entry<TValue> (item).State = state;
    }
}