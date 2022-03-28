using System.ComponentModel.DataAnnotations;

namespace StarApp1.Models
{
    public class LoginViewModel
    {
        [Required]
        public int UserId   { get; set; }

        [Required]
        [Key]
        [EmailAddress(ErrorMessage ="Enter Incedo Email")]
        public string UserName { get; set; }
       
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
        public int RoleId { get; set; }
        public int StatusId { get; set; }
        public string Name { get; set; }


    }
}
