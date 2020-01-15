using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon_RedJags.Models
{
    
        public class TrainingProgram : IValidatableObject
    {
            [Required]
            public int Id { get; set; }

            [Required(ErrorMessage = "Training Program Name is required")]
            [Display(Name = "Training Name")]
            [StringLength(255, MinimumLength = 1, ErrorMessage = "Training Program Name length should be between 1 and 255 characters")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Start Date of Training Program is required")]
            [DataType(DataType.DateTime)]
            public DateTime StartDate { get; set; }

            [Required(ErrorMessage = "End Date of Training Program is required")]
            [DataType(DataType.DateTime)]
            public DateTime EndDate { get; set; }

            [Required(ErrorMessage = "Maximum Number of Attendees is required for Training Program")]
            public int MaxAttendees { get; set; }

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                List<ValidationResult> results = new List<ValidationResult>();

                if (StartDate < DateTime.Now)
                {
                    results.Add(new ValidationResult("Start date and time must be greater than current time", new[] { "StartDateTime" }));
                }

                if (EndDate <= StartDate)
                {
                    results.Add(new ValidationResult("EndDateTime must be greater that StartDateTime", new[] { "EndDateTime" }));
                }

                return results;
            }
    }
    


}
