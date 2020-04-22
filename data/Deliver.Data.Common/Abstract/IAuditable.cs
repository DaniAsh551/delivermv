using System;
using System.Collections.Generic;
using System.Text;

namespace Deliver.Data.Common
{
    public interface IAuditable
    {
        DateTime CreationTime { get; set; }
        DateTime ModificationTime { get; set; }
        DateTime DeletedTime { get; set; }
        int? CreatedUserId { get; set; }
        int? ModifiedUserId { get; set; }
        int? DeletedUserId { get; set; }
        bool IsActive { get; set; }
    }
}
