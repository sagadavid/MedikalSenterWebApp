using System.ComponentModel.DataAnnotations;

namespace MedikalSenter.Models
{
    public class Doctor
    {
       
        public int ID { get; set; }

        //LEP//summary property, derive a formal name
        [Display(Name = "Doctor")]
        public string FullName
        {
            get
            {
                return "Dr. " + FirstName
                    + (string.IsNullOrEmpty(MiddleName) ? " " :
                        (" " + (char?)MiddleName[0] + ". ").ToUpper())
                    + LastName;
            }
        }

        [Display(Name = "Doctor")]
        public string FormalName
        {
            get
            {
                return LastName + ", " + FirstName
                    + (string.IsNullOrEmpty(MiddleName) ? "" :
                        (" " + (char?)MiddleName[0] + ".").ToUpper());
            }
        }



        [Display(Name = "First Name")]
        [Required(ErrorMessage ="dont leave first name blank")]
        [StringLength(50, ErrorMessage ="first name, not more than 50 char")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        [StringLength(50, ErrorMessage = "middle name no more than 50 char")]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "dont leave last name blank")]
        [StringLength(100, ErrorMessage = "last name, not more than 100 char")]
        public string LastName { get; set; }

        //LEP//navigation property, constructed with hashset empty collection 
        public ICollection<Patient> Patients { get; set; } = new HashSet<Patient>();
        //public Doctor(){ this.Patients = new HashSet<Patient>();}
    }
}
