using System.ComponentModel.DataAnnotations;

namespace MedikalSenter.Models
{
    public class Doctor
    {
        //public Doctor(){ this.Patients = new HashSet<Patient>();}
       
        public int ID { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage ="dont leave first name blank")]
        [StringLength(50, ErrorMessage ="first name, not more than 50 char")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        [Required(ErrorMessage = "dont leave middle name blank")]
        [StringLength(50, ErrorMessage = "middle name no more than 50 char")]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "dont leave last name blank")]
        [StringLength(100, ErrorMessage = "last name, not more than 100 char")]
        public string LastName { get; set; }

        public ICollection<Patient> Patients { get; set; } = new HashSet<Patient>();

    }
}
