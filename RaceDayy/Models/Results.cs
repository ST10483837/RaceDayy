namespace RaceDayy.Models
{
    public class Results
    {
        public int ResultsID { get; set; }
        public TimeSpan? FinishTime { get; set; }
        public int? Position { get; set; }
        public string? Pace { get; set; }
        public string RaceStatus { get; set; } = "Pending";
        public int EnrollmentID { get; set; }
        public Enrollment? Enrollment { get; set; }
    }
}