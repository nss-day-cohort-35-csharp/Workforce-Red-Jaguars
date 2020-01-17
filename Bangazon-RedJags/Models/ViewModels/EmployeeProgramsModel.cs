using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon_RedJags.Models.ViewModels
{
    public class EmployeeProgramsModel
    {
        //public Employee Employee { get; set; }

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int ProgramId { get; set; } = 0;
        public List<SelectListItem> AssignedPrograms { get; set; }
        public List<SelectListItem> EligiblePrograms { get; set; }
    }
}
