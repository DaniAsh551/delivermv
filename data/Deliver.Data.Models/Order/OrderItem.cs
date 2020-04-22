using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Deliver.Data.Models
{
    public class OrderItem : CommonProps<int>
    {
        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [Required]
        public string OrderDetails { get; set; }

        public string Notes { get; set; }

    }
}
