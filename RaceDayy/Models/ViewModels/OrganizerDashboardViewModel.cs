namespace RaceDayy.Models.ViewModels
{
    public class OrganizerDashboardViewModel
    {
        public int TotalEvents { get; set; }
        public int TotalEnrollments { get; set; }
        public int TotalCategories { get; set; }
        public List<EventSummary> Events { get; set; } = new List<EventSummary>();
    }

    public class EventSummary
    {
        public int EventID { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int CategoryCount { get; set; }
        public int EnrollmentCount { get; set; }
    }
}