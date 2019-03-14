using System.Linq;
using Generic.Repository.Models.BaseEntities.BasePagination.BasePage;

namespace Generic.Repository.Extensions.Page
{
    /// <summary>
    /// Extension method to paginate entity E
    /// </summary>
    public static class Page
    {
        /// <summary>
        /// Paginate entity E, default values: pageStartInOne: false, sort= ASC, order=Id, size=10
        /// </summary>
        /// <param name="listEntities">IQueryable from entity E</param>
        /// <param name="config">Config from Page</param>
        /// <typeparam name="E">Entity E</typeparam>
        /// <returns>Paginated List E</returns>
        public static Page<E> ToPage<E>(this IQueryable<E> listEntities, BaseConfigurePage config)
        where E : class => ToPage<E>(listEntities, config, false);

        /// <summary>
        /// Paginate entity E, default values: sort= ASC, order=Id, size=10
        /// </summary>
        /// <param name="listEntities">IQueryable from entity E</param>
        /// <param name="config">Config from Page</param>
        /// <param name="pageStartInOne">If Page starts on index 1</param>
        /// <typeparam name="E"></typeparam>
        /// <returns></returns>
        public static Page<E> ToPage<E>(this IQueryable<E> listEntities, BaseConfigurePage config, bool pageStartInOne)
        where E : class => ToPage<E>(listEntities, config, pageStartInOne, "ASC");

        /// <summary>
        /// Paginate entity E, default values: order=Id, size=10
        /// </summary>
        /// <param name="listEntities">IQueryable from entity E</param>
        /// <param name="config">Config from Page</param>
        /// <param name="pageStartInOne">If Page starts on index 1</param>
        /// <param name="defaultSort">Default value to sort ("ASC" or "DESC")</param>
        /// <typeparam name="E"></typeparam>
        /// <returns></returns>
        public static Page<E> ToPage<E>(this IQueryable<E> listEntities, BaseConfigurePage config, bool pageStartInOne, string defaultSort)
        where E : class => ToPage<E>(listEntities, config, pageStartInOne, defaultSort, "Id");

        /// <summary>
        /// Paginate entity E, default values: size=10
        /// </summary>
        /// <param name="listEntities">IQueryable from entity E</param>
        /// <param name="config">Config from Page</param>
        /// <param name="pageStartInOne">If Page starts on index 1</param>
        /// <param name="defaultSort">Default value to sort ("ASC" or "DESC")</param>
        /// <param name="defaultOrder">Default value to order (Name property)</param>>
        /// <typeparam name="E"></typeparam>
        /// <returns></returns>
        public static Page<E> ToPage<E>(this IQueryable<E> listEntities, BaseConfigurePage config, bool pageStartInOne, string defaultSort, string defaultOrder)
        where E : class => ToPage<E>(listEntities, config, pageStartInOne, defaultSort, defaultOrder, 10);

        /// <summary>
        /// Paginate entity E, no default values
        /// </summary>
        /// <param name="listEntities">IQueryable from entity E</param>
        /// <param name="config">Config from Page</param>
        /// <param name="pageStartInOne">If Page starts on index 1</param>
        /// <param name="defaultSort">Default value to sort ("ASC" or "DESC")</param>
        /// <param name="defaultOrder">Default value to order (Name property)</param>>
        /// <param name="defaultSize">Default value to size</param>
        /// <typeparam name="E"></typeparam>
        /// <returns></returns>
        public static Page<E> ToPage<E>(this IQueryable<E> listEntities, BaseConfigurePage config, bool pageStartInOne, string defaultSort, string defaultOrder, int defaultSize)
        where E : class => new Page<E>(listEntities, config, pageStartInOne, defaultSort, defaultOrder, defaultSize);
    }
}