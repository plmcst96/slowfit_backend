namespace slowfit.DTOResponse
{
    public class PersonalTrainerResponse
    {
        public int UserId { get; set; }

        public string FirstName { get; set; } = null!;

        public string Surname { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }

        public string? Province { get; set; }

        public int? ZipCode { get; set; }

        public DateTime? BirthDate { get; set; }

        public int? RoleId { get; set; }

        public string? ImageProfile { get; set; }

        public string? Phone { get; set; }

        // Dati fiscali
        public string? VatNumber { get; set; }

        public string? FiscalCode { get; set; }

        public string? SdiCode { get; set; }

        public string? PecEmail { get; set; }

        // Id dei clienti collegati al PT
        public IReadOnlyList<int> ClientIds { get; set; } = new List<int>();

        // Esito dell'invio dell'email di attivazione: valorizzato solo in fase di creazione.
        // null quando non applicabile (liste / dettaglio; il re-invio comunica l'esito via status code).
        public bool? EmailSent { get; set; }
    }
}
