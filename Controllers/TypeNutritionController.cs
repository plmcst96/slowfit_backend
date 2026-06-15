using System;
using Microsoft.AspNetCore.Mvc;
using slowfit.DBModels;
using slowfit.DTORequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace slowfit.Controllers
{
    [Route("slowFit/typeNutrition")]
    [ApiController]
    public class TypeNutritionController(SlowFitContext slowFitCtx) : ControllerBase
    {

        private readonly SlowFitContext _slowFitContext = slowFitCtx;


        [HttpGet]
        public ActionResult<IEnumerable<TypeNutritionRes>> GetAll()
        {
            var typeList = new List<TypeNutritionRes>();
            try
            {

                typeList = [.. _slowFitContext.TypeNutritions.Select(n => new TypeNutritionRes
                {
                    TypeNutritionId = n.TypeNutritionId,
                    TypeNutritionName = n.TypeNutritionName,
                })];

                if (typeList.Count == 0) return NoContent();

                return Ok(typeList);
            }
            catch (Exception)
            {
                return BadRequest($"An error occurred");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<TypeNutritionRes> GetById(int id)
        {
            try
            {
                var typeNutrition = _slowFitContext.TypeNutritions
                    .Where(n => n.TypeNutritionId == id)
                    .Select(n => new TypeNutritionRes
                    {
                        TypeNutritionId = n.TypeNutritionId,
                        TypeNutritionName = n.TypeNutritionName,
                    })
                    .FirstOrDefault();

                if (typeNutrition == null)
                    return NotFound($"TypeNutrition with ID {id} not found.");

                return Ok(typeNutrition);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }



        [HttpPost]
        public IActionResult Post([FromBody] TypeNutritionRes typeN)
        {
            if (typeN == null)
            {
                return BadRequest("Data type incorrect");
            }


            if (string.IsNullOrEmpty(typeN.TypeNutritionName))
            {
                return BadRequest();
            }
            try
            {
                var type = new TypeNutrition()
                {
                    TypeNutritionName = typeN.TypeNutritionName,
                };
                _slowFitContext.TypeNutritions.Add(type);
                _slowFitContext.SaveChanges();
                return Ok("Nutritional Plan created successfully.");
            }
            catch (Exception)
            {
                return BadRequest($"Failed to create nutritional plan");
            }
        }

       
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] TypeNutritionRes typeN)
        {
            if (typeN == null || typeN.TypeNutritionId != id)
            {
                return BadRequest("Invalid data or ID does not match");
            }

            var existingType = _slowFitContext.TypeNutritions.Where(n => n.TypeNutritionId == id).FirstOrDefault();
            if (existingType == null)
            {
                return NotFound("Plan not found");
            }

            // Aggiorna i dati
            existingType.TypeNutritionName = typeN.TypeNutritionName;


            try
            {
                _slowFitContext.TypeNutritions.Update(existingType);


                _slowFitContext.SaveChanges();

                return Ok("Nutritional Plan updated succesfully");
            }
            catch (Exception)
            {
                return BadRequest($"Failed to update nutritional plan: {typeN.TypeNutritionName}");
            }
        }

       
        [HttpDelete("{id}")] 
        public IActionResult Delete(int id)
        {
            var typeN = _slowFitContext.TypeNutritions.Where(n => n.TypeNutritionId == id).FirstOrDefault();

            if (typeN == null)
            {
                return NotFound(new { message = "Plan not found" });
            }

            try
            {
                _slowFitContext.TypeNutritions.Remove(typeN);
                _slowFitContext.SaveChanges();
                return Ok("Plan delete succesfully");
            }
            catch (Exception)
            {
                return BadRequest($"Error to delete plan {typeN.TypeNutritionName} ");
            }

        }

    }
}
