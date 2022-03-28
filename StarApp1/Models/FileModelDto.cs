namespace StarApp1.Models
{
    public class FileModelDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Resoursename { get; set; }
        public string ResourseId { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public string Hours { get; set; }
        public string ApprovalStatus { get; set; }
        public string TimesheetNumber { get; set; }
        public string Vertical { get; set; }
        public string Horizontal { get; set; }
        public string SubHorizontal { get; set; }
        public string CoustmerId { get; set; }
        public string CoustmerName { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
    }
}
