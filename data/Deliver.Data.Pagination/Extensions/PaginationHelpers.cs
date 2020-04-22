using Deliver.Data.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deliver.Data.Pagination.Extensions
{
    public static class PaginationHelpers
    {
        /// <summary>
        /// Get Pagination object for a query
        /// </summary>
        /// <typeparam name="T">Type of Resultset</typeparam>
        /// <param name="query">The query to be executed</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of elements per page</param>
        /// <returns>API Pagination</returns>
        private static Paging GetPagination<T>(IQueryable<T> query, int page = 1, int pageSize = 10)
        {
            return new Paging(page, pageSize, query.Count());
        }

        /// <summary>
        /// Get Pagination object for an Enumerable
        /// </summary>
        /// <typeparam name="T">Type of Resultset</typeparam>
        /// <param name="collection">The Enumerable to be used</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of elements per page</param>
        /// <returns>API Pagination</returns>
        private static Paging GetPagination<T>(IEnumerable<T> collection, int page = 1, int pageSize = 10)
        {
            return new Paging(page, pageSize, collection.Count());
        }

        /// <summary>
        /// Get the result of a pagination applied to a query
        /// </summary>
        /// <typeparam name="T">Type of Resultset</typeparam>
        /// <param name="query">The query to be executed</param>
        /// <param name="paging">The pagination object to be used</param>
        /// <returns>Paginated result</returns>
        private static T[] GetPaginatedResult<T>(IQueryable<T> query, Paging paging)
        {
            int skip = ((paging?.Page ?? 1) * (paging?.PageSize ?? 10)) - (paging?.PageSize ?? 10);
            return query.Skip(skip).Take(paging.PageSize).ToArray();
        }

        /// <summary>
        /// Get the result of a pagination applied to a query asynchronously
        /// </summary>
        /// <typeparam name="T">Type of Resultset</typeparam>
        /// <param name="collection">The collection to be used</param>
        /// <param name="paging">The pagination object to be used</param>
        /// <returns>Paginated result</returns>
        private static T[] GetPaginatedResult<T>(IEnumerable<T> collection, Paging paging)
        {
            int skip = ((paging?.Page ?? 1) * (paging?.PageSize ?? 10)) - (paging?.PageSize ?? 10);
            return collection?.Skip(skip)?.Take(paging.PageSize)?.ToArray();
        }

        /// <summary>
        /// Get the result of a pagination applied to a query asynchronously and set the Service's paging
        /// </summary>
        /// <typeparam name="T">Type of Resultset</typeparam>
        /// <param name="query">The query to be executed</param>
        /// <param name="service">The service where the pagination should be set (If calling from the service just pass 'this')</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of elements per page</param>
        /// <returns>Paginated result</returns>
        public static T[] Paginate<T>(this IQueryable<T> query, IHasPaging service, int page = 1, int pageSize = 10)
        {
            service.Paging = GetPagination<T>(query, page, pageSize);
            return GetPaginatedResult<T>(query, service.Paging);
        }

        /// <summary>
        /// Get the result of a pagination applied to a collection asynchronously and set the Service's paging
        /// </summary>
        /// <typeparam name="T">Type of Resultset</typeparam>
        /// <param name="query">The collection to be paginated</param>
        /// <param name="service">The service where the pagination should be set (If calling from the service just pass 'this')</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of elements per page</param>
        /// <returns>Paginated result</returns>
        public static T[] Paginate<T>(this IEnumerable<T> collection, IHasPaging service, int page = 1, int pageSize = 10)
        {
            service.Paging = GetPagination<T>(collection, page, pageSize);
            return GetPaginatedResult<T>(collection, service.Paging);
        }
    }
}
