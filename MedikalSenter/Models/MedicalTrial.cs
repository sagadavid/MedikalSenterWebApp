using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MedikalSenter.Models
{
    public class MedicalTrial
    {
        public int ID { get; set; }

        [Display(Name = "Trial Name")]
        [Required(ErrorMessage = "You cannot leave the name of the trial blank.")]
        [StringLength(100, ErrorMessage = "Trial name cannot be more than 100 characters long.")]
        [DataType(DataType.MultilineText)]
        [DisplayFormat(NullDisplayText = "None")]
        public string TrialName { get; set; }
        public virtual ICollection<Patient> Patients { get; set; } = new HashSet<Patient>();
    }
}
