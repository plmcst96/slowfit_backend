using Microsoft.AspNetCore.Mvc;
using slowfit.DBModels;
using slowfit.DTORequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace slowfit.Controllers
{
    [Route("slowFit/typePlan")]
    [ApiController]
    public class TypePlanController(SlowFitContext slowFitCtx) : ControllerBase
    {
        
        private readonly SlowFitContext _slowFitContext = slowFitCtx;

    
        [HttpGet]
        public ActionResult<IEnumerable<TypePlanRes>> GetAll()
        {
            var planList = new List<TypePlanRes>();
            try
            {

                planList = _slowFitContext.TypePlans.Select(p => new TypePlanRes
                {
                    TypePlaneId = p.TypePlaneId,
                    TypePlaneName = p.TypePlaneName,
                }).ToList();

                if (planList.Count == 0) return NoContent();

                return Ok(planList);
            }
            catch (Exception ex)
            {
                return BadRequest( $"An error occurred");
            }
        }

    
        [HttpPost]
        public IActionResult Post([FromBody] TypePlanRes typeP)
        {
            if (typeP == null)
            {
                return BadRequest("Data plan incorrect");
            }

            if (string.IsNullOrEmpty(typeP.TypePlaneName) || typeP.TypePlaneId == 0)
            {
                return BadRequest();
            }
            try
            {
                var plan = new TypePlan()
                {
                    TypePlaneName = typeP.TypePlaneName,
                };
                _slowFitContext.TypePlans.Add(plan);
                _slowFitContext.SaveChanges();
                return Ok("Plan created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create plan");
            }
        }

      
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] TypePlanRes typeP)
        {
            if (typeP == null || typeP.TypePlaneId != id)
            {
                return BadRequest("Invalid data or ID does not match");
            }

            var existingType = _slowFitContext.TypePlans.Where(u => u.TypePlaneId == id).FirstOrDefault();
            if (existingType == null)
            {
                return NotFound("Plan not found");
            }

            // Aggiorna i dati
            existingType.TypePlaneName = typeP.TypePlaneName;


            try
            {
                _slowFitContext.TypePlans.Update(existingType);


                _slowFitContext.SaveChanges();

                return Ok("Plan updated succesfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update plan: {typeP.TypePlaneName}");
            }
        }

       
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var typeP = _slowFitContext.TypePlans.Where(u => u.TypePlaneId == id).FirstOrDefault();

            if (typeP == null)
            {
                return NotFound(new { message = "Plan not found" });
            }

            try
            {
                _slowFitContext.TypePlans.Remove(typeP);
                _slowFitContext.SaveChanges();
                return Ok("Plan delete succesfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error to delete plan {typeP.TypePlaneName} ");
            }

            
        }
    }
}
