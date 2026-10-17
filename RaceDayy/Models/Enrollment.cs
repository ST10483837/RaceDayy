namespace RaceDayy.Models
{
    public class Enrollment
    {
        public int EnrollmentID { get; set; }
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Registered";
        public string PaymentStatus { get; set; } = "Pending";
        public int ParticipantID { get; set; }
        public Participant? Participant { get; set; }
        public int CategoryID { get; set; }
        public Category? Category { get; set; }
        public Results? Result { get; set; }
    }
}