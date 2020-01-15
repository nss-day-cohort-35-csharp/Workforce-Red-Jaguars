using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon_RedJags.Models.ViewModels
{
    public class DepartmentEmployeesCount
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Budget { get; set; }

        [Display(Name = "Number of Employees")]
        public int EmployeeNumber { get; set; }
    }
}
