namespace slowfit.DTORequest
{
    public class UserRegister
    {

        public string FirstName { get; set; } = null!;

        public string Surname { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public int RoleId { get; set; }

        public int? PtId { get; set; }

    }
}
