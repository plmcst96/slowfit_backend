namespace slowfit.DTOResponse
{
    public class UserProfile
    {
        public int UserId { get; set; }

        public string FirstName { get; set; } = null!;

        public string Surname { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; } 

        public string? Province { get; set; } 

        public DateTime? BirthDate { get; set; }

        public int? ZipCode { get; set; }

        public int? RoleId { get; set; }

        public string? ImageProfile { get; set; }

        public string? Phone { get; set; }
    }
}