using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MedikalSenter.Models
{
    public class Patient
    {
        public int ID { get; set; }

        //adding summary properties here..
        //collect some props and make a new/derived for custom purposes
        //notmapped is not needed, there are only getters.

        [Display(Name = "Patient")]
        public string FullName
        {
            get
            {
                return FirstName
                    + (string.IsNullOrEmpty(MiddleName) ? " " 
                    : (" " + (char?)MiddleName[0] + ". ").ToUpper())//ternary operator here
                    + LastName;
            }
        }

        public string Age//dont provide bithday easily which is private data, so give age only??
        {
            get
            {
                DateTime today = DateTime.Today;
                int? a = today.Year - DOB?.Year
                    - ((today.Month < DOB?.Month || 
                    (today.Month == DOB?.Month && today.Day < DOB?.Day) ? 1 : 0));
                return a?.ToString(); 
                /*Note: You could add .PadLeft(3) but spaces disappear in a web page. */
            }
        }

        [Display(Name = "Phone")]
        public string PhoneFormatted
        {//string format, needs substring to excavate
            get
            {
                return "(" + Phone.Substring(0, 3) + ") " 
                    + Phone.Substring(3, 3) + "-" + Phone[6..];
            }
        }

        [Required(ErrorMessage = "dont leave OHIP blank")]
        [RegularExpression("^\\d{10}$", ErrorMessage ="OHIP is exact 10 digits")]//only ten digits
        [StringLength(10)]//we include this to limit the size of database field to 10 digits
        public string OHIP { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "dont leave first name blank")]
        [StringLength(50, ErrorMessage = "first name, not more than 50 char")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        [StringLength(50, ErrorMessage = "middle name no more than 50 char")]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "dont leave last name blank")]
        [StringLength(100, ErrorMessage = "last name, not more than 100 char")]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]//chrome needs formatting
        public DateTime? DOB { get; set; }

        [Display(Name = "Visits/Yr")]
        [Required(ErrorMessage = "You cannot leave the number of expected vists per year blank.")]
        [Range(1, 12, ErrorMessage = "The number of expected vists per year must be between 1 and 12.")]//notice range
        public byte ExpYrVisits { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression("^\\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number (no spaces).")]
        [DataType(DataType.PhoneNumber)]//notice special datatype
        [StringLength(10)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email Address is required.")]
        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        public string EMail { get; set; }

        //gonna build one to many relation
        [Required(ErrorMessage = "You must select a Primary Care Physician.")]
        [Display(Name = "Doctor")]
        public int DoctorID { get; set; }
        //navigation prop
        public Doctor Doctor { get; set; }


    }


}
