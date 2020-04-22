using Deliver.Data.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Deliver.Data.Models
{
    public class CommonProps<T> : ICommonProps<T>
    {
        public CommonProps()
        {
            IsActive = true;
            CreationTime = DateTime.Now;
            ModificationTime = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        [ScaffoldColumn(false)]
        public T Id { get; set; }

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public DateTime CreationTime { get; set; }
        [ScaffoldColumn(false)]
        [JsonIgnore]
        public DateTime ModificationTime { get; set; }
        [ScaffoldColumn(false)]
        [JsonIgnore]
        public DateTime DeletedTime { get; set; }
        [ScaffoldColumn(false)]
        [JsonIgnore]
        public int? CreatedUserId { get; set; }
        [ScaffoldColumn(false)]
        [JsonIgnore]
        public int? ModifiedUserId { get; set; }
        [ScaffoldColumn(false)]
        [JsonIgnore]
        public int? DeletedUserId { get; set; }
        [ScaffoldColumn(false)]
        [JsonIgnore]
        public bool IsActive { get; set; }

        public T GetId()
            => this.Id;

        object ICommonProps.GetId()
            => this.Id;
    }
}
