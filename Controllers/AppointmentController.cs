using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using slowfit.DBModels;
using slowfit.DTORequest;
using slowfit.DTOResponse;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace slowfit.Controllers
{
    [Route("slowFit/appointment")]
    [ApiController]
    public class AppointmentController(SlowFitContext slowFitContext) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitContext;

        // GET: api/<AppointmentController>
        [HttpGet]
        public ActionResult<IEnumerable<AppointmentRes>> Get()
        {
            var appointmentList = new List<AppointmentRes>();
            try
            {

                appointmentList = _slowFitContext.Appointments.Select(t => new AppointmentRes
                {
                    AppointmentId = t.AppointmentId,
                    Date = DateTime.ParseExact(t.Date, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                    PtId = t.PtId,
                    Description = t.Description,
                    Duration = t.Duration,
                    UserId = t.UserId,
                    CallUrl = t.CallUrl

                }).ToList();


                if (appointmentList.Count == 0) return NoContent();

                return Ok(appointmentList);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred");
            };
        }

        // GET api/<AppointmentController>/5
        [HttpGet("{id}")]
        public ActionResult<AppointmentResponse> GetSingleAppointment(int id)
        {
            try
            {
                var appointment = _slowFitContext.Appointments
                    .Where(t => t.AppointmentId == id)
                    .Join(_slowFitContext.Users,
                          a => a.UserId,
                          u => u.UserId,
                          (a, u) => new AppointmentResponse
                          {
                              AppointmentId = a.AppointmentId,
                              Date = DateTime.ParseExact(a.Date, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                              PtId = a.PtId,
                              Description = a.Description,
                              Duration = a.Duration,
                              CallUrl = a.CallUrl,
                              UserId = a.UserId,
                              UserFullName = u.FirstName + " " + u.Surname,
                              UserEmail = u.Email,
                              UserPhone = u.Phone

                          })
                    .FirstOrDefault();
                if (appointment == null) return NotFound();
                return Ok(appointment);
            }
            catch (Exception ex)
            {
                return BadRequest($"No appointment found with {id}");
            }
        }

        [HttpGet("byUser/{userId}")]
        public ActionResult<AppointmentRes> GetAppointmentByUserId(int userId)
        {
            try
            {
                var appointment = _slowFitContext.Appointments.Where(a => a.UserId == userId).FirstOrDefault();
                if (appointment == null) return NotFound();
                return Ok(appointment);
            }
            catch (Exception ex)
            {
                return BadRequest($"No appointment found for {userId}");
            }
        }

        [HttpGet("byDate/{date}")]
        public ActionResult<IEnumerable<AppointmentRes>> GetAppointmentByDate(DateTime date)
        {
            try
            {
                var appointments = _slowFitContext.Appointments
                    .Where(a => DateTime.ParseExact(a.Date, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).Date == date.Date)
                    .Select(a => new AppointmentRes
                    {
                        AppointmentId = a.AppointmentId,
                        Date = DateTime.ParseExact(a.Date, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                        PtId = a.PtId,
                        Description = a.Description,
                        Duration = a.Duration,
                        UserId = a.UserId,
                        CallUrl = a.CallUrl
                    })
                    .ToList();

                return appointments.Any() ? Ok(appointments) : NoContent();
            }
            catch
            {
                return BadRequest($"No appointment found for {date}");
            }
        }

        [HttpGet("byPT/{ptId}")]
        public ActionResult<IEnumerable<AppointmentResponse>> GetAppointmentByPtId(int ptId)
        {
            try
            {
                var appointmentList = _slowFitContext.Appointments
                    .Where(a => a.PtId == ptId)
                    .Join(_slowFitContext.Users,
                          a => a.UserId,
                          u => u.UserId,
                          (a, u) => new AppointmentResponse
                          {
                              AppointmentId = a.AppointmentId,
                              Date = DateTime.ParseExact(a.Date, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                              PtId = a.PtId,
                              Description = a.Description,
                              Duration = a.Duration,
                              CallUrl = a.CallUrl,
                              UserId = a.UserId,
                              UserFullName = u.FirstName + " " + u.Surname,
                              UserEmail = u.Email,
                              UserPhone = u.Phone

                          })
        
                    .ToList();

                return appointmentList.Any() ? Ok(appointmentList) : NoContent();
            }
            catch
            {
                return BadRequest("An error occurred");
            }
        }



        // POST api/<AppointmentController>
        [HttpPost]
        public IActionResult CreateAppointment([FromBody] AppointmentRes appointment)
        {
            if (appointment == null)
            {
                return BadRequest("Invalid request body.");
            }

            Console.WriteLine($"CallUrl Received: {appointment.CallUrl}"); // DEBUG

            if (appointment.PtId <= 0 ||
                string.IsNullOrEmpty(appointment.Description) ||
                appointment.UserId <= 0 ||
                appointment.Duration <= 0 || string.IsNullOrEmpty(appointment.CallUrl))
            {
                return BadRequest();
            }

            try
            {
                var app = new Appointment()
                {
                    Date = appointment.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                    PtId = appointment.PtId,
                    Description = appointment.Description,
                    Duration = appointment.Duration,
                    UserId = appointment.UserId,
                    CallUrl = appointment.CallUrl // Verifica se qui è ancora null
                };

                Console.WriteLine($"CallUrl Saved in DB: {app.CallUrl}"); // DEBUG

                _slowFitContext.Appointments.Add(app);
                _slowFitContext.SaveChanges();
                return Ok(new { message = "Appointment created successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to create appointment");
            }
        }


        // PUT api/<AppointmentController>/5
        [HttpPut("{id}")]
        public IActionResult UpdateAppointment(int id, [FromBody] AppointmentRes updateApp)
        {
            var appointment = _slowFitContext.Appointments.Where(t => t.AppointmentId == id).FirstOrDefault();
            if (appointment == null) return NotFound();

            appointment.Date = updateApp.Date.ToString("yyyy-MM-dd HH:mm:ss");
            appointment.PtId = updateApp.PtId;
            appointment.Description = updateApp.Description;
            appointment.CallUrl = updateApp.CallUrl;
            appointment.Duration = updateApp.Duration;
            appointment.UserId = updateApp.UserId;
            try
            {
                _slowFitContext.Appointments.Update(appointment);


                _slowFitContext.SaveChanges();

                return Ok(new { message = "Appointment updated succesfully" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update appointment: {updateApp.AppointmentId}");
            }
        }

        // DELETE api/<AppointmentController>/5
        [HttpDelete("{id}")]
        public IActionResult DeleteAppointment(int id)
        {
            var app = _slowFitContext.Appointments.Where(t => t.AppointmentId == id).FirstOrDefault();
            if (app == null) return NotFound();

            try
            {
                _slowFitContext.Appointments.Remove(app);
                _slowFitContext.SaveChanges();

                return Ok($"The appointment has been successfully cancelled");
            }
            catch (Exception ex)
            {
                return BadRequest($"No appointment found whit {id}");
            }
        }
    }
}
