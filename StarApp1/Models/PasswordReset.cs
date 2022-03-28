using System.ComponentModel.DataAnnotations;

namespace StarApp1.Models
{
    public class PasswordReset
    {
        [Required]
        [Key]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@(incedoinc)\\.(com)$", ErrorMessage = "Please use INCEDO EmailId")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Password \"{0}\" must have {2} character", MinimumLength = 8)]
        [RegularExpression(@"^([a-zA-Z0-9@*#]{8,15})$",
        ErrorMessage = "Password must contain: Minimum 8 characters atleast 1 UpperCase Alphabet, 1 LowerCase Alphabet, 1 Number and 1 Special Character")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password should be same as earlier")]
        public string ConfirmPassword { get; set; }
        public int Otp { get; set; }

    }
}
