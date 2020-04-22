using Deliver.Data.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Deliver.Data.Models
{
    public class OrderPaymentMethod : CommonProps<int>
    {
        [Required]
        public string OrderId { get; set; }
        public Order Order { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }
    }
}
