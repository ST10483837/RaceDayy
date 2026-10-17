namespace RaceDayy.Models
{
    public class Event
    {
        public int EventID { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Status { get; set; } = "Upcoming";
        public string EventType { get; set; } = "Run";
        public double Distance { get; set; }
        public int OrganizerID { get; set; }
        public Organizer? Organizer { get; set; }
        public int VenueID { get; set; }
        public Venue? Venue { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}