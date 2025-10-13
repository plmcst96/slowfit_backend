namespace slowfit.DTOResponse
{
    public class UserRegisterResponse
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
