using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarApp1.Models
{
    public class AllowanceDashboardViewModel
    {
        public string Name  { get; set; }
        [Key]
        public int SAPid { get; set; }
        public int Hours { get; set; }
        public int LeaveHours { get; set; }
        public int AfternoonShiftDays { get; set; }
        public int NightShiftDays { get; set; }
        public int TotalDays { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid Input")]
        public int TransportAllowance { get; set; }
        public int TotalAllowance   { get; set; }
        public string ApprovalStatus { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Invalid Input")]
        public int NightShiftAllowance { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid Input")]
        public int AfternoonShiftAllowance { get; set; }

        
        public string ProjectId { get; set; }
        [NotMapped]
        //[Required(ErrorMessage ="Please select the Project Name")]
        public List<SelectListItem> Project { get; set; }
        
        public string PeriodStart { get; set; }

        [NotMapped]
        //[Required(ErrorMessage ="Please Select the Period")]
        public List<SelectListItem> Period{ get; set; }

        public IFormFile browseFile { get; set; }
        public int LogId { get; internal set; }
    }
}
