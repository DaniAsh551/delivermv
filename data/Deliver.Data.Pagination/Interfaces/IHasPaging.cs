using System;
using System.Collections.Generic;
using System.Text;

namespace Deliver.Data.Pagination
{
    public interface IHasPaging
    {
        Paging Paging { get; set; }
    }
}
