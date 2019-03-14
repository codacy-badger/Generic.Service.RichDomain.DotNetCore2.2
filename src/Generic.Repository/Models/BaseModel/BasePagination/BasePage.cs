using System;
using System.Collections.Generic;
using System.Linq;
using Generic.Repository.Extensions.Commom;

namespace Generic.Repository.Models.BaseEntities.BasePagination.BasePage
{
    /// <summary>
    /// Page Class
    /// Default values:
    /// sort = ASC
    /// order = Id
    /// </summary>
    /// <typeparam name="E">Type of Page</typeparam>
    public class Page<E>
        where E : class
    {
        #region Default Parameters
        private readonly bool _pageStatsInOne;
        private readonly string _defaultSort;
        private readonly string _defaultOrder;
        private readonly int _defaultSize;
        #endregion
        #region Parameters Ctor
        private readonly BaseConfigurePage _config;
        private readonly IQueryable<E> _listEntities;
        private readonly int _count;
        #endregion

        #region Ctor
        public Page(IQueryable<E> listEntities, BaseConfigurePage config, bool pageStartInOne, string defaultSort, string defaultOrder, int defaultSize)
        {
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

        private void ValidateCtor(int count, IQueryable<E> listEntities, BaseConfigurePage config)
        {
            if (count < 1 || config == null)
                throw new Exception($"ERROR> NameClass: {nameof(ValidateCtor)}. {Environment.NewLine}Message: The {(config != null ? nameof(listEntities) : nameof(config))} is empty!");
        }

        public IEnumerable<E> Content
        {
            get
            {
                var queryableE =  _listEntities.Skip(NumberPage * TotalElements).Take(Size);
                return Sort == "ASC" ? queryableE.OrderBy(x => Commom.CacheGet[typeof(E).Name][Order](x)).ToList() 
                : queryableE.OrderByDescending(x => Commom.CacheGet[typeof(E).Name][Order](x)).ToList();
            }
        }

        public int TotalElements
        {
            get => _count;
        }

        public string Sort
        {
            get => _config.sort ?? _defaultSort;
        }

        public string Order
        {
            get => _config.order ?? _defaultOrder;
        }

        public int Size
        {
            get => _config.size == 0 ? _defaultSize : _config.size;
        }

        public int NumberPage
        {
            get => _pageStatsInOne ? _config.page - 1 : _config.page;
        }
    }
}