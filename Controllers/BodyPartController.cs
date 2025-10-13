using Microsoft.AspNetCore.Mvc;
using slowfit.DBModels;
using slowfit.DTORequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace slowfit.Controllers
{
    [Route("slowFit/bodypart")]
    [ApiController]
    public class BodyPartController(SlowFitContext slowFitCtx) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitCtx;


        [HttpGet]
        public ActionResult<IEnumerable<BodyPartRes>> GetAll()
        {
            var bodyList = new List<BodyPartRes>();
            try
            {

                bodyList = _slowFitContext.BodyParts.Select(p => new BodyPartRes
                {
                    
                    BodyPartId= p.BodyPartId,
                    BodyPartName = p.BodyPartName,
                }).ToList();

                if (bodyList.Count == 0) return NoContent();

                return Ok(bodyList);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred");
            }
        }


        [HttpPost]
        public IActionResult Post([FromBody] BodyPartRes bodyP)
        {
            if (bodyP == null)
            {
                return BadRequest("Data body part incorrect");
            }

            if (string.IsNullOrEmpty(bodyP.BodyPartName) || bodyP.BodyPartId == 0)
            {
                return BadRequest();
            }
            try
            {
                var body = new BodyPart() 
                {
                    BodyPartName = bodyP.BodyPartName
                };
                _slowFitContext.BodyParts.Add(body);
                _slowFitContext.SaveChanges();
                return Ok("body part created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create body part");
            }
        }


        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] BodyPartRes bodyP)
        {
            if (bodyP == null || bodyP.BodyPartId != id)
            {
                return BadRequest("Invalid data or ID does not match");
            }

            var existingBody = _slowFitContext.BodyParts.Where(u => u.BodyPartId == id).FirstOrDefault();
            if (existingBody == null)
            {
                return NotFound("Plan not found");
            }

            // Aggiorna i dati
            existingBody.BodyPartName = bodyP.BodyPartName;


            try
            {
                _slowFitContext.BodyParts.Update(existingBody);


                _slowFitContext.SaveChanges();

                return Ok("Body part updated succesfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update body part: {bodyP.BodyPartName}");
            }
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var bodyP = _slowFitContext.BodyParts.Where(u => u.BodyPartId == id).FirstOrDefault();

            if (bodyP == null)
            {
                return NotFound(new { message = "Body part not found" });
            }

            try
            {
                _slowFitContext.BodyParts.Remove(bodyP);
                _slowFitContext.SaveChanges();
                return Ok("Body plan delete succesfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error to delete body part {bodyP.BodyPartName} ");
            }


        }
    }
}
