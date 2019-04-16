using Generic.Service.Extensions.Commom;
using Generic.Service.Extensions.Filter;
using Generic.Service.Extensions.Validation;
using Generic.Service.Models.BaseModel.Filter;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Generic.Service.Service.Base
{
    ///<summary>
    /// This is a Base Service implementation which any entity or Service should be have.
    /// TValue - Entity Type, F inheritance of IFilter or IFilter
    ///</summary>
    public abstract class BaseService<TValue, TFilter> : IBaseService<TValue, TFilter>
        where TValue : class
    where TFilter : class, IFilter
    {
#region Attr
        private TValue item;
        protected readonly DbContext _context;
        private readonly string _includeDateNameField;
        private readonly bool _useCommit;
#endregion

#region Ctor
        protected BaseService(DbContext context)
        {
            _context = context;
            _useCommit = false;
            _includeDateNameField = "dateInclusion";

        }

        protected BaseService(DbContext context, bool useCommit)
        {
            _includeDateNameField = "dateInclusion";
            _useCommit = useCommit;
            _context = context;
        }

        protected BaseService(DbContext context, string includeDateNameField)
        {
             _includeDateNameField.IsNull(nameof(BaseService<TValue, TFilter>), nameof(_includeDateNameField));
            _useCommit = false;
            _context = context;
        }

        protected BaseService(DbContext context, string includeDateNameField, bool useCommit)
        {
            _includeDateNameField.IsNull(nameof(BaseService<TValue, TFilter>), nameof(_includeDateNameField));
            _useCommit = useCommit;
            _context = context;
        }
#endregion

#region QUERY
        public virtual IQueryable<TValue> GetAll(bool EnableAsNoTracking) => EnableAsNoTracking ? _context.Set<TValue>().AsNoTracking() : _context.Set<TValue>();

        public virtual IQueryable<TValue> GetAllBy(Expression<Func<TValue, bool>> predicate, bool EnableAsNoTracking) => predicate != null ?
            GetAll(EnableAsNoTracking).Where(predicate) : GetAll(EnableAsNoTracking);

        public virtual IQueryable<TValue> FilterAll(TFilter filter, bool EnableAsNoTracking) => GetAllBy(filter.GenerateLambda<TValue, TFilter>(), EnableAsNoTracking);

        public virtual async Task<TValue> GetByAsync(Expression<Func<TValue, bool>> predicate, bool EnableAsNoTracking) => !predicate.IsNull(nameof(GetByAsync), nameof(predicate)) &&
        EnableAsNoTracking ? await _context.Set<TValue>().AsNoTracking().SingleOrDefaultAsync(predicate) : await _context.Set<TValue>().SingleOrDefaultAsync(predicate);
#endregion

#region COMMAND - (CREAT, UPDATE, DELETE) With CancellationToken
        public virtual async Task<TValue> CreateAsync(CancellationToken token)
        {
            SetState(EntityState.Added, item);
            if (!_useCommit)
            {
                await CommitAsync(token).ConfigureAwait(false);
            }
            return item;
        }

        public virtual async Task UpdateAsync(CancellationToken token)
        {
            SetState(EntityState.Modified, item);
            if (!_useCommit)
            {
                await CommitAsync(token).ConfigureAwait(false);
            }
        }    

        public virtual async Task DeleteAsync(CancellationToken token)
        {
            _context.Remove(ReturnE());
            if (!_useCommit)
            {
                await CommitAsync(token).ConfigureAwait(false);
            }
        }
#endregion

#region COMMAND - (CREAT, UPDATE, DELETE) Without CancellationToken
        public virtual async Task<TValue> CreateAsync()=> await CreateAsync(default(CancellationToken)).ConfigureAwait(false);

        public virtual async Task UpdateAsync()=> await UpdateAsync(default(CancellationToken)).ConfigureAwait(false);

        public virtual async Task DeleteAsync() => await DeleteAsync(default(CancellationToken)).ConfigureAwait(false);
#endregion

#region public Methods 
        public void Map(TValue item)
        {
            SetThis(item);
        }
#endregion

#region COMMIT
        public Task CommitAsync() => CommitAsync(default(CancellationToken));

        public Task CommitAsync(CancellationToken cancellationToken) => _context.SaveChangesAsync(cancellationToken);
#endregion

#region private Methods
        private void SetThis(TValue item)
        {
            this.item = ReturnE();
            Commom.CacheGet[typeof(TValue).Name].ToList().ForEach(get =>
            {
                if (Commom.CacheSet[typeof(TValue).Name].TryGetValue(get.Key, out Action<object, object> set))
                {
                    if (get.Key.Equals(_includeDateNameField))
                    {
                        set(this.item, DateTime.Now);
                    }
                    else
                    {
                        set(this.item, get.Value(item));
                    }
                }
            });
        }
        
        private TValue ReturnE() => (TValue)Convert.ChangeType(this, typeof(TValue));

        private void SetState(EntityState state, TValue item) => _context.Attach(item).State = state;        
#endregion

    }
}