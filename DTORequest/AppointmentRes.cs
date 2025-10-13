namespace slowfit.DTORequest
{
    public class AppointmentRes
    {
        public int AppointmentId { get; set; }

        public DateTime Date { get; set; }

        public int PtId { get; set; }

        public int Duration { get; set; }

        public string? Description { get; set; }

        public string? CallUrl { get; set; }

        public int UserId { get; set; }
    }
}
