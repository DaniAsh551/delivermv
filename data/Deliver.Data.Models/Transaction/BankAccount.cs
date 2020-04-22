using Deliver.Data.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Deliver.Data.Models
{
    public class BankAccount : CommonProps<int>
    {
        [Required]
        public Bank Bank { get; set; }
        [Required]
        [MaxLength(13)]
        [MinLength(13)]
        [RegularExpression("[0-9]*", ErrorMessage = "Please provide a valid Account Number")]
        public string Number { get; set; }
    }
}
