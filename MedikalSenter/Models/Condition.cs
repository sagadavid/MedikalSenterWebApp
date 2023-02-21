using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MedikalSenter.Models
{
    public class Condition
    {
        public int ID { get; set; }

        [Display(Name = "Medical Condition")]
        [Required(ErrorMessage = "You cannot leave the name of the condition blank.")]
        [StringLength(50, ErrorMessage = "Too Big!")]
        public string ConditionName { get; set; }

        [Display(Name = "Medical History")]
        public ICollection<PatientCondition> PatientConditions { get; set; } = new HashSet<PatientCondition>();
    }
}
