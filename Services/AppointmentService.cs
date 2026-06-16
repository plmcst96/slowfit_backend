using Microsoft.EntityFrameworkCore;
using slowfit.DBModels;
using slowfit.DTORequest;
using slowfit.DTOResponse;

namespace slowfit.Services;

public interface IAppointmentService : ICrudService<AppointmentRes>
{
    Task<ServiceResult<AppointmentResponse>> GetResponseByIdAsync(int id);
    Task<ServiceResult<IReadOnlyList<AppointmentResponse>>> GetByUserAsync(int userId);
    Task<ServiceResult<IReadOnlyList<AppointmentRes>>> GetByDateAsync(DateTime date);
    Task<ServiceResult<IReadOnlyList<AppointmentResponse>>> GetByPtAsync(int ptId);
}

public sealed class AppointmentService(SlowFitContext context) : CrudServiceBase<Appointment, AppointmentRes>(context), IAppointmentService
{
    protected override DbSet<Appointment> Set => ((SlowFitContext)DbContext).Appointments;
    protected override string EntityCode => "appointment";
    protected override string EntityName => "Appointment";
    protected override int GetId(Appointment entity) => entity.AppointmentId;
    protected override int GetDtoId(AppointmentRes dto) => dto.AppointmentId;
    protected override AppointmentRes ToDto(Appointment entity) => new() { AppointmentId = entity.AppointmentId, Date = entity.Date, PtId = entity.PtId, Description = entity.Description, Duration = entity.Duration, UserId = entity.UserId, CallUrl = entity.CallUrl };
    protected override Appointment CreateEntity(AppointmentRes dto) => new() { Date = dto.Date, PtId = dto.PtId, Description = dto.Description!, Duration = dto.Duration, UserId = dto.UserId, CallUrl = dto.CallUrl! };
    protected override void UpdateEntity(Appointment entity, AppointmentRes dto)
    {
        entity.Date = dto.Date;
        entity.PtId = dto.PtId;
        entity.Description = dto.Description!;
        entity.Duration = dto.Duration;
        entity.UserId = dto.UserId;
        entity.CallUrl = dto.CallUrl!;
    }
    protected override bool IsValid(AppointmentRes dto) => dto != null && dto.PtId > 0 && dto.UserId > 0 && dto.Duration > 0 && !string.IsNullOrWhiteSpace(dto.Description) && !string.IsNullOrWhiteSpace(dto.CallUrl);

    public async Task<ServiceResult<AppointmentResponse>> GetResponseByIdAsync(int id)
    {
        var appointment = await ResponseQuery().FirstOrDefaultAsync(a => a.AppointmentId == id);
        return appointment == null ? ServiceResult<AppointmentResponse>.Ok(default!) : ServiceResult<AppointmentResponse>.Ok(appointment);
    }

    public async Task<ServiceResult<IReadOnlyList<AppointmentResponse>>> GetByUserAsync(int userId)
    {
        var appointments = await ResponseQuery().Where(a => a.UserId == userId).ToListAsync();
        return ServiceResult<IReadOnlyList<AppointmentResponse>>.Ok(appointments);
    }

    public async Task<ServiceResult<IReadOnlyList<AppointmentRes>>> GetByDateAsync(DateTime date)
    {
        var entities = await Set.AsNoTracking().Where(a => a.Date.Date == date.Date).ToListAsync();
        var appointments = entities.Select(ToDto).ToList();
        return ServiceResult<IReadOnlyList<AppointmentRes>>.Ok(appointments);
    }

    public async Task<ServiceResult<IReadOnlyList<AppointmentResponse>>> GetByPtAsync(int ptId)
    {
        var appointments = await ResponseQuery().Where(a => a.PtId == ptId).ToListAsync();
        return ServiceResult<IReadOnlyList<AppointmentResponse>>.Ok(appointments);
    }

    private IQueryable<AppointmentResponse> ResponseQuery() =>
        from appointment in ((SlowFitContext)DbContext).Appointments.AsNoTracking()
        join user in ((SlowFitContext)DbContext).Users.AsNoTracking() on appointment.UserId equals user.UserId
        select new AppointmentResponse
        {
            AppointmentId = appointment.AppointmentId,
            Date = appointment.Date,
            PtId = appointment.PtId,
            Description = appointment.Description,
            Duration = appointment.Duration,
            CallUrl = appointment.CallUrl,
            UserId = appointment.UserId,
            UserFullName = user.FirstName + " " + user.Surname,
            UserEmail = user.Email,
            UserPhone = user.Phone
        };
}
