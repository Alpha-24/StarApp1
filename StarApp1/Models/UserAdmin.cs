using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarApp1.Models
{
    public class UserAdmin
    {
        
        [Key]
        public string UserName { get; set; }
        public string Name { get; set; }
        public string ActiveFrom { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public string Confirmation { get; set; }
        [NotMapped]
        public List<SelectListItem> RoleName { get; set;}
        public int RoleId { get; set; }


    }
}
