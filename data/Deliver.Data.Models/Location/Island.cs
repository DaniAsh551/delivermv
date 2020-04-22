using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Deliver.Data.Models
{
    public class Island : CommonProps<int>
    {
        public Island()
        {
            Shops = new HashSet<IslandShop>();
            Orders = new HashSet<Order>();
        }

        [Required]
        public string Name { get; set; }

        
        public ICollection<IslandShop> Shops { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
