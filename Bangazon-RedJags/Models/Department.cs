using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon_RedJags.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Department Name is required")]
        [StringLength(15, MinimumLength = 2, ErrorMessage = "Department Name should be between 2 and 15 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Department budget is required")]
        [Range(10, 99999999, ErrorMessage = "Budget should be between 10 and 99999999")]
        public int Budget { get; set; }

        
    }
}
