using slowfit.DTORequest;

namespace slowfit.DTOResponse
{
    public class AppointmentResponse : AppointmentRes
    {

        public string? UserFullName { get; set; }

        public string? UserEmail { get; set; } = null;

        public string? UserPhone { get; set; } = null;


    }
}
