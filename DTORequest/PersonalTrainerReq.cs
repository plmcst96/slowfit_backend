namespace slowfit.DTORequest
{
    public class PersonalTrainerReq
    {
        public string FirstName { get; set; } = null!;

        public string Surname { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }

        public string? Province { get; set; }

        public int? ZipCode { get; set; }

        public DateTime? BirthDate { get; set; }

        public string? ImageProfile { get; set; }

        public string? Phone { get; set; }

        // Dati fiscali
        public string? VatNumber { get; set; }

        public string? FiscalCode { get; set; }

        public string? SdiCode { get; set; }

        public string? PecEmail { get; set; }
    }
}
