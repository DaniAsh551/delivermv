using Deliver.Data.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Deliver.Data.Models
{
    public class UserPaymentMethod : CommonProps<int>
    {
        [Required]
        public string UserId { get; set; }
        public User User { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }
    }
}
