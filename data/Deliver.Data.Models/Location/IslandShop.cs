using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Deliver.Data.Models
{
    public class IslandShop : CommonProps<int>
    {
        [Required]
        public int IslandId { get; set; }
        public Island Island { get; set; }

        [Required]
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
