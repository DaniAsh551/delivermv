using Deliver.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Deliver.Web.ViewModels
{
    public class UpdateOrderVm
    {
        public decimal? Price { get; set; }
        public string Notes { get; set; }
        public OrderStatus? Status { get; set; }
    }
}
