using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon_RedJags.Models.ViewModels
{
    public class EmployeeViewModel
    {
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        [Display(Name = "Last name")]
        public string LastName { get; set; }
        [Display(Name = "Department")]
        public string DepartmentName { get; set; }
        
        //
        public Employee Employee { get; set; }
        public List<SelectListItem> Departments { get; set; }

    }
}
