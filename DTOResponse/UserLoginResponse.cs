namespace slowfit.DTOResponse
{
    public class UserLoginResponse
    {
        
        public String? Email { get; set; }
        public String? Message { get; set; }

        public int? UserId { get; set; }

        public int? RoleId { get; set; }

        public string? Token { get; set; }

    }
}
