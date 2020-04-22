using Deliver.Data.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Deliver.Data.Models
{
    public class Order : CommonProps<string>
    {
        public Order()
        {
            Status = OrderStatus.Received;
            OrderItems = new HashSet<OrderItem>();
            PaymentMethods = new HashSet<OrderPaymentMethod>();
        }

        [Required]
        public string Address { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }

        [Required]
        public int IslandId { get; set; }
        public Island Island { get; set; }

        [Required]
        [RegularExpression("[0-9]*")]
        [StringLength(7, MinimumLength =7)]
        public string PhoneNumber { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        public string Notes { get; set; }
        public decimal? Price { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<OrderPaymentMethod> PaymentMethods { get; set; }
    }
}
