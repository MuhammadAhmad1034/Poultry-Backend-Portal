using System.ComponentModel.DataAnnotations;

namespace PoultryPro_Portal.Models
{
    public class AdminModel
    {
        [Required]
        [Display(Name = "Email Address")]
        public string adminEmail { get; set; }

        [Required]
        [Display(Name = "Password")]

        [DataType(DataType.Password)]
        public string password { get; set; }
    }
}
