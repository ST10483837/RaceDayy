using RaceDayy.Data;

namespace RaceDayy.Models
{
    public class Organizer
    {
        public int OrganizerID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
    }
}