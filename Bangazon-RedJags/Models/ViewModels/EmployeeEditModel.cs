using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon_RedJags.Models.ViewModels
{
    public class EmployeeEditModel
    {
        public Employee Employee { get; set; }

        public List<SelectListItem> Departments { get; set; }
        //public List<SelectListItem> IsSupervisor { get; set; }
        public List<SelectListItem> Computers { get; set; }
    }
}
