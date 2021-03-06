using Generic.Service.Extensions.Commom;
using Generic.Service.Extensions.Validation;
using Generic.Service.Models.BaseModel.Page.PageConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Generic.Service.Models.BaseModel.Page
{
    public abstract class AbstractPage<TValue, TResult> : IPage<TResult>
        where TValue : class
    where TResult : class
    {
        #region Default Parameters
        protected readonly Func<IEnumerable<TValue>, IEnumerable<TResult>> _mapperTo;
        protected readonly bool _pageStatsInOne;
        protected readonly string _defaultSort;
        protected readonly string _defaultOrder;
        protected readonly int _defaultSize;
        #endregion
        #region Parameters Ctor
        protected readonly IPageConfiguration _config;
        protected readonly IQueryable<TValue> _listEntities;
        protected readonly int _count;
        #endregion

        #region Ctor
        protected AbstractPage(IQueryable<TValue> listEntities, Func<IEnumerable<TValue>, IEnumerable<TResult>> mapperTo, IPageConfiguration config, bool pageStartInOne, string defaultSort, string defaultOrder, int defaultSize)
        {
            _mapperTo = mapperTo;
            _count = listEntities.Count();
            ValidateCtor(_count, listEntities, config);
            _config = config;
            _listEntities = listEntities;
            _pageStatsInOne = pageStartInOne;
            _defaultOrder = defaultOrder;
            _defaultSize = defaultSize;
            _defaultSort = defaultSort;
        }
        #endregion

        private static void ValidateCtor(int count, IQueryable<TValue> listEntities, IPageConfiguration config)
        {
            config.IsNull(nameof(ValidateCtor), nameof(config));
            if (count < 1)
            {
                ValidateObject.HandleNullError($"ClassName: {nameof(ValidateCtor)} {Environment.NewLine}Message: The {nameof(listEntities)} is empty!");
            }
        }
        public bool Equals(TResult other)
        {
            other.IsNull(nameof(Equals),nameof(other));
            return other == this;
        }

        public virtual List<TResult> Content
        {
            get
            {
                IQueryable<TValue> queryableE = Sort == "ASC" ? _listEntities.OrderBy(x => Commom.CacheGet[typeof(TValue).Name][Order](x)) :
                    _listEntities.OrderByDescending(x => Commom.CacheGet[typeof(TValue).Name][Order](x));
                queryableE = queryableE.Skip(NumberPage * Size).Take(Size);

                return _mapperTo(queryableE).ToList();
            }
        }

        public virtual int TotalElements
        {
            get => _count;
        }

        public virtual string Sort
        {
            get => _config.sort ?? _defaultSort;
        }

        public virtual string Order
        {
            get => _config.order ?? _defaultOrder;
        }

        public virtual int Size
        {
            get => _config.size == 0 ? _defaultSize : _config.size;
        }

        public virtual int NumberPage
        {
            get => _pageStatsInOne ? _config.page - 1 : _config.page;
        }

        public virtual int TotalPage
        {
            get => TotalElements / Size;
        }
    }

    public abstract class AbstractPage<TValue> : AbstractPage<TValue, TValue>
        where TValue : class
    {
        #region Ctor
        protected AbstractPage(IQueryable<TValue> listEntities, IPageConfiguration config, bool pageStartInOne, string defaultSort, string defaultOrder, int defaultSize) : base(listEntities, null, config, pageStartInOne, defaultSort, defaultOrder, defaultSize) { }
        #endregion

        public override List<TValue> Content
        {
            get
            {
                IQueryable<TValue> queryableE = Sort == "ASC" ? _listEntities.OrderBy(x => Commom.CacheGet[typeof(TValue).Name][Order](x)) :
                    _listEntities.OrderByDescending(x => Commom.CacheGet[typeof(TValue).Name][Order](x));
                queryableE = queryableE.Skip(NumberPage * Size).Take(Size);

                return queryableE.ToList();
            }
        }

    }
}