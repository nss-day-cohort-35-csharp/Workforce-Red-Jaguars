using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon_RedJags.Models
{
    public class Computer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Purchase Date is required")]
        [DataType(DataType.DateTime)]
        public DateTime PurchaseDate { get; set; }

        public DateTime? DecomissionDate { get; set; } = null;

        [Required(ErrorMessage = "Computer Manufacturer is required")]
        [Display(Name = "Manufacturer")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Manufacturer Name length should be between 1 and 20 characters")]
        public string Make { get; set; }

        [Required(ErrorMessage = "Computer Model is required")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Model Name length should be between 1 and 20 characters")]
        public string Model { get; set; }
    }
}
