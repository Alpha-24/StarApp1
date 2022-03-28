using System.ComponentModel.DataAnnotations;

namespace StarApp1.Controllers
{
    public class UpdateViewModel
    {
        [Key]
        public int LogId { get; set; }

        public uint updatedASD { get; set; }
        public uint updatedNSD { get; set; }
        public uint updatedTSD { get; set; }
        public uint Hours { get; set; }

    }
}