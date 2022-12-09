using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MedikalSenter.Models
{
    public class Patient
    {

        public int ID { get; set; }

        [Required(ErrorMessage = "dont leave OHIP blank")]
        [RegularExpression("^\\d{10}$", ErrorMessage ="OHIP is exact 10 digits")]
        [StringLength(10)]//we include this to limit the size of database field to 10 digits
        public string OHIP { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "dont leave first name blank")]
        [StringLength(50, ErrorMessage = "first name, not more than 50 char")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        [Required(ErrorMessage = "dont leave middle name blank")]
        [StringLength(50, ErrorMessage = "middle name no more than 50 char")]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "dont leave last name blank")]
        [StringLength(100, ErrorMessage = "last name, not more than 100 char")]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DOB { get; set; }

        [Display(Name = "Visits/Yr")]
        [Required(ErrorMessage = "You cannot leave the number of expected vists per year blank.")]
        [Range(1, 12, ErrorMessage = "The number of expected vists per year must be between 1 and 12.")]
        public byte ExpYrVisits { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression("^\\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number (no spaces).")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(10)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email Address is required.")]
        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        public string EMail { get; set; }

        [Required(ErrorMessage = "You must select a Primary Care Physician.")]
        [Display(Name = "Doctor")]
        public int DoctorID { get; set; }

        public Doctor Doctor { get; set; }


    }


}
