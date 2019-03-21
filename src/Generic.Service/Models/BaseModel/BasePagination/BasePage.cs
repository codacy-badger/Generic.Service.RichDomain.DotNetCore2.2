using System;
using System.Collections.Generic;
using System.Linq;
using Generic.Service.Extensions.Commom;
using Generic.Service.Models.BaseModel.BasePagination;
using Generic.Service.Models.BaseModel.BasePagination.BaseConfigurePage;

namespace Generic.Service.Models.BaseEntities.BasePagination {

    /// <summary>
    /// Page Class
    /// Default values:
    /// sort = ASC
    /// order = Id
    /// </summary>
    /// <typeparam name="TValue">Type of page return</typeparam>
    public class Page<TValue> : AbstractPage<TValue>
        where TValue : class {
            public Page (IQueryable<TValue> listEntities, IBaseConfigurePage config, bool pageStartInOne, string defaultSort, string defaultOrder, int defaultSize) 
            : base (listEntities, config, pageStartInOne, defaultSort, defaultOrder, defaultSize) { }
        }

    public class Page<TValue, TResult> : AbstractPage<TValue, TResult>
        where TValue : class
    where TResult : class {
        public Page (IQueryable<TValue> listEntities, Func<IEnumerable<TValue>, IEnumerable<TResult>> mapperTo, IBaseConfigurePage config, bool pageStartInOne, string defaultSort, string defaultOrder, int defaultSize) : base (listEntities, mapperTo, config, pageStartInOne, defaultSort, defaultOrder, defaultSize) { }
    }
}