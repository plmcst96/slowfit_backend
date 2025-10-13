namespace slowfit.DTORequest
{
    public class UserRes
    {
        public new int UserId { get; set; }

        public string FirstName { get; set; } = null!;

        public string Surname { get; set; } = null!;

        public string Email { get; set; } = null!;

        public int? RoleId { get; set; }

        public int? PtId { get; set; }

        public string? ImageProfile { get; set; }

        public string? Phone { get; set; }

    }
}
