namespace slowfit.DTORequest
{
    public class AddProfile 
    {
        public int UserId { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; } 

        public string? Country { get; set; } 

        public string? Province { get; set; } 

        public DateTime? BirthDate { get; set; }

        public int ZipCode { get; set; }

        public string? ImageProfile { get; set; }

        public string? Phone { get; set; }
    }
}
