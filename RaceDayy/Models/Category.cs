using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayy.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(5,2)")]
        public decimal DistanceKm { get; set; }

        public string? AgeGroup { get; set; }
        public string? GenderRestriction { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal EntryFee { get; set; }

        public int? MaxParticipants { get; set; }
        public int EventID { get; set; }
        public Event? Event { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}