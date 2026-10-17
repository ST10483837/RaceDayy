namespace RaceDayy.Models
{
    public class Venue
    {
        public int VenueID { get; set; }
        public string VenueName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = "South Africa";
        public int? Capacity { get; set; }

        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}