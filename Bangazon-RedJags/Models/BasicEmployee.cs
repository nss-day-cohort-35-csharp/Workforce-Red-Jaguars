using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon_RedJags.Models
{
    public class BasicEmployee
    {
        public int Id { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Department Name")]

        public string DepartmentName { get; set; }

        [Display(Name = "Computer Name")]

        public string Computer { get; set; }

        public List<String> EmployeeTrainings { get; set; } = new List<String>();

        //public List<SelectListItem> TrainingList { get; set; }

        public List<TrainingSelect> TrainingList { get; set; }

        
    }
}
