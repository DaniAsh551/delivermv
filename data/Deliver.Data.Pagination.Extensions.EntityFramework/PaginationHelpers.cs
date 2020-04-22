using Deliver.Data.Pagination;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore
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
        /// <returns>Api Pagination</returns>
        private static Paging GetPagination<T>(IQueryable<T> query, int page = 1, int pageSize = 10)
        {
            return new Paging(page, pageSize, query.Count());
        }

        /// <summary>
        /// Get the result of a pagination applied to a query asynchronously
        /// </summary>
        /// <typeparam name="T">Type of Resultset</typeparam>
        /// <param name="query">The query to be executed</param>
        /// <param name="paging">The pagination object to be used</param>
        /// <returns>Paginated result</returns>
        private static async Task<T[]> GetPaginatedResultAsync<T>(IQueryable<T> query, Paging paging)
        {
            int skip = ((paging?.Page ?? 1) * (paging?.PageSize ?? 10)) - (paging?.PageSize ?? 10);
            return await query.Skip(skip).Take(paging.PageSize).ToArrayAsync();
        }
        
        /// <summary>
        /// Get the result of a pagination applied to a query asynchronously
        /// </summary>
        /// <typeparam name="T">Type of Resultset</typeparam>
        /// <param name="query">The queries to be executed</param>
        /// <param name="paging">The pagination object to be used</param>
        /// <returns>Paginated result</returns>
        private static async Task<T[]> GetPaginatedResultAsync<T>(IEnumerable<IQueryable<T>> queries, Paging paging, params int[] counts)
        {
            var sum = counts.Sum();

            int skip = ((paging?.Page ?? 1) * (paging?.PageSize ?? 10)) - (paging?.PageSize ?? 10);

            var queryIndex = 0;
            IQueryable<T> activeQuery()
                => queries.Skip(queryIndex).FirstOrDefault();

            while(skip >= counts[queryIndex])
            {
                skip -= counts[queryIndex];
                queryIndex++;
            }

            var items = await activeQuery().Skip(skip).Take(paging.PageSize).ToArrayAsync();
            while(items.Length < (paging?.PageSize ?? 10) && queryIndex < (counts.Length - 1))
            {
                queryIndex++;
                //No. of additional items needed from next query to complete the page
                var needed = (paging?.PageSize ?? 10) - items.Length;
                var additionalItems = await activeQuery().Take(needed).ToArrayAsync();
                var newItems = new T[items.Length + additionalItems.Length];
                Array.Copy(items, newItems, items.Length);
                Array.Copy(additionalItems, 0, newItems, items.Length, additionalItems.Length);

                items = newItems;
            }

            return items;
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
        /// Get the result of a pagination applied to a query asynchronously and set the Service's paging asynchronously
        /// </summary>
        /// <typeparam name="T">Type of Resultset</typeparam>
        /// <param name="query">The query to be executed</param>
        /// <param name="service">The service where the pagination should be set (If calling from the service just pass 'this')</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of elements per page</param>
        /// <returns>Paginated result</returns>
        public static async Task<T[]> PaginateAsync<T>(this IQueryable<T> query, IHasPaging service, int page = 1, int pageSize = 10)
        {
            service.Paging = new Paging(page, pageSize, await query.CountAsync());
            return await GetPaginatedResultAsync<T>(query, service.Paging);
        }

        /// <summary>
        /// Get the result of a pagination applied to a query asynchronously and set the Service's paging asynchronously
        /// </summary>
        /// <typeparam name="T">Type of Resultset</typeparam>
        /// <param name="query">The query to be executed</param>
        /// <param name="service">The service where the pagination should be set (If calling from the service just pass 'this')</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of elements per page</param>
        /// <returns>Paginated result</returns>
        public static async Task<T[]> PaginateAsync<T>(this IIncludableQueryable<T, object> query, IHasPaging service, int page = 1, int pageSize = 10)
        {
            service.Paging = new Paging(page, pageSize, await query.CountAsync());
            return await GetPaginatedResultAsync<T>(query, service.Paging);
        }

        /// <summary>
        /// Get the result of a pagination applied to a query asynchronously and set the Service's paging asynchronously
        /// </summary>
        /// <typeparam name="T">Type of Resultset</typeparam>
        /// <param name="query">The query to be executed</param>
        /// <param name="service">The service where the pagination should be set (If calling from the service just pass 'this')</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of elements per page</param>
        /// <returns>Paginated result</returns>
        public static async Task<T[]> PaginateAsync<T>(this IOrderedQueryable<T> query, IHasPaging service, int page = 1, int pageSize = 10)
        {
            service.Paging = new Paging(page, pageSize, await query.CountAsync());
            return await GetPaginatedResultAsync<T>(query, service.Paging);
        }

        public static async Task<T[]> PaginateAsync<T>(this IEnumerable<IQueryable<T>> queries, IHasPaging service, int page = 1, int pageSize = 10)
        {
            var counts = await Task.WhenAll(queries.Select(async x => await x.CountAsync()).ToArray());
            var sum = counts.Sum();

            service.Paging = new Paging(page, pageSize, sum);
            return await GetPaginatedResultAsync(queries, service.Paging, counts);
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


        /// <summary>
        /// Get Pagination object for an Enumerable
        /// </summary>
        /// <typeparam name="T">Type of Resultset</typeparam>
        /// <param name="collection">The Enumerable to be used</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of elements per page</param>
        /// <returns>Api Pagination</returns>
        private static Paging GetPagination<T>(IEnumerable<T> collection, int page = 1, int pageSize = 10)
        {
            return new Paging(page, pageSize, collection.Count());
        }

        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, int? page = 1, int? pageSize = 10) where T : class
        {
            int skip = (pageSize.Value * page.Value) - pageSize.Value;
            return queryable?.Skip(skip)?.Take(pageSize.Value);
        }
    }
}
