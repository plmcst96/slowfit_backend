using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using slowfit.DBModels;
using slowfit.DTORequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace slowfit.Controllers
{
    [Route("slowFit/location")]
    [ApiController]
    public class LocationTrainingController(SlowFitContext slowFitCtx) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitCtx;

        // GET: api/<LocationTrainingController>
        [HttpGet]
        public ActionResult<IEnumerable<LocationTrainingRes>> GetAll()
        {
            var locationList = new List<LocationTrainingRes>();
            try
            {

                locationList = _slowFitContext.LocationTrainings.Select(n => new LocationTrainingRes
                {
                   
                    LocationId = n.LocationId,
                    LocationString = n.LocationString,
                }).ToList();

                if (locationList.Count == 0) return NoContent();

                return Ok(locationList);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred");
            }
        
}
        

        // POST api/<LocationTrainingController>
        [HttpPost]
        public IActionResult CreateLocationTraining([FromBody] LocationTrainingRes location)
        {
            if (location == null)
            {
                return BadRequest("Data type incorrect");
            }


            if (string.IsNullOrEmpty(location.LocationString))
            {
                return BadRequest();
            }
            try
            {
                var type = new LocationTraining()
                {
                    LocationString = location.LocationString,
                 

                };
                _slowFitContext.LocationTrainings.Add(type);
                _slowFitContext.SaveChanges();
                return Ok("Location training created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create Location training");
            }
        }

        // PUT api/<LocationTrainingController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] LocationTrainingRes updateLocation)
        {
            if (updateLocation == null || updateLocation.LocationId != id)
            {
                return BadRequest("Invalid data or ID does not match");
            }

            var existingType = _slowFitContext.LocationTrainings.Where(n => n.LocationId == id).FirstOrDefault();
            if (existingType == null)
            {
                return NotFound("Plan not found");
            }

            // Aggiorna i dati
            existingType.LocationString = updateLocation.LocationString;
           
            try
            {
                _slowFitContext.LocationTrainings.Update(existingType);


                _slowFitContext.SaveChanges();

                return Ok("Location training updated succesfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update Location training: {existingType.LocationString}");
            }
        }

        // DELETE api/<LocationTrainingController>/5
        [HttpDelete("{id}")]
        public IActionResult DeleteLocalTraining(int id)
        {
            var location = _slowFitContext.LocationTrainings.Where(n => n.LocationId == id).FirstOrDefault();
            if (location == null)
            {
                return NotFound("Location not found");
            }
            try
            {
               
                _slowFitContext.LocationTrainings.Remove(location);
                _slowFitContext.SaveChanges();
                return Ok("Location training deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to delete location training: {location.LocationString}");
            }
        }
    }
}
