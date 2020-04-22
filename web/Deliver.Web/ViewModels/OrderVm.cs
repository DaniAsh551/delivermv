using Deliver.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Deliver.Web.ViewModels
{
    public class OrderVm
    {
        public string Address { get; set; }
        public string Store { get; set; }
        public int Island { get; set; }
        public string Phone { get; set; }
        public PaymentMethod[] PaymentMethods { get; set; }
        public string[] OrderItems { get; set; }
    }
}
