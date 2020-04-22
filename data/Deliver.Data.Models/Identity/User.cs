using Deliver.Data.Common;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Deliver.Data.Models
{
    public class User :  IdentityUser, ICommonProps<string>
    {
        public User()
        {
            Islands = new HashSet<IslandShop>();
            Orders = new HashSet<Order>();
            PaymentMethods = new HashSet<UserPaymentMethod>();
            IsActive = true;
        }

        #region CommonProps
        [ScaffoldColumn(false)]
        public DateTime CreationTime { get; set; }
        [ScaffoldColumn(false)]
        public DateTime ModificationTime { get; set; }
        [ScaffoldColumn(false)]
        public DateTime DeletedTime { get; set; }
        [ScaffoldColumn(false)]
        public int? CreatedUserId { get; set; }
        [ScaffoldColumn(false)]
        public int? ModifiedUserId { get; set; }
        [ScaffoldColumn(false)]
        public int? DeletedUserId { get; set; }
        [ScaffoldColumn(false)]
        public bool IsActive { get; set; }

        object ICommonProps.GetId()
            => this.Id;

        public string GetId()
            => this.Id;
        #endregion

        [Required]
        public UserType UserType { get; set; }

        [Required]
        public string Name { get; set; }


        public string BmlAccount { get; set; }
        public string MibAccount { get; set; }

        public virtual ICollection<UserPaymentMethod> PaymentMethods { get; set; }
        public virtual ICollection<IslandShop> Islands { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
