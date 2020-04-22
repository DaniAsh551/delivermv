using System;

namespace Deliver.Data.Pagination
{
    public class Paging
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / (double)PageSize);
        public bool IsFirstPage => Page <= 1;
        public bool IsLastPage => !HasPageAfter;
        public bool HasPageBefore => Page > 1;
        public bool HasPageAfter => Page < TotalPages;
        
        public Paging()
        {

        }

        public Paging(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }

        public Paging(int page, int pageSize, int totalCount)
        {
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
        }
    }
}
