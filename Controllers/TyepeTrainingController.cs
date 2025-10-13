using System;
using Microsoft.AspNetCore.Mvc;
using slowfit.DBModels;
using slowfit.DTORequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace slowfit.Controllers
{
    [Route("slowFit/type")]
    [ApiController]
    public class TyepeTrainingController(SlowFitContext slowFitCtx) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitCtx;

       
        [HttpGet]
        public ActionResult<IEnumerable<TypeTrainingRes>> GetAll()
        {
            var typeList = new List<TypeTrainingRes>();
            try
            {

                typeList = _slowFitContext.TypeTrainigs.Select(p => new TypeTrainingRes
                {
                    TypeId = p.TypeId,
                    TypeName = p.TypeName,
                }).ToList();

                if (typeList.Count == 0) return NoContent();

                return Ok(typeList);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<TypeTrainingRes> GetById(int id)
        {
            try
            {
                var type = _slowFitContext.TypeTrainigs
                    .Where(p => p.TypeId == id)
                    .Select(p => new TypeTrainingRes
                    {
                        TypeId = p.TypeId,
                        TypeName = p.TypeName,
                    })
                    .FirstOrDefault();

                if (type == null)
                {
                    return NotFound($"No training type found with ID {id}");
                }

                return Ok(type);
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred while retrieving the training type.");
            }
        }


        [HttpPost]
        public IActionResult Post([FromBody] TypeTrainingRes typeT)
        {
            if (typeT == null)
            {
                return BadRequest("Data type incorrect");
            }

            if (string.IsNullOrEmpty(typeT.TypeName))
            {
                return BadRequest();
            }
            try
            {
                var type = new TypeTrainig()
                {
                    TypeName = typeT.TypeName,
                };
                _slowFitContext.TypeTrainigs.Add(type);
                _slowFitContext.SaveChanges();
                return Ok("Type training created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create type");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] TypeTrainingRes typeT)
        {
            if (typeT == null || typeT.TypeId != id)
            {
                return BadRequest("Invalid data or ID does not match");
            }

            var existingType = _slowFitContext.TypeTrainigs.Where(u => u.TypeId == id).FirstOrDefault();
            if (existingType == null)
            {
                return NotFound("Plan not found");
            }

            // Aggiorna i dati
            existingType.TypeName = typeT.TypeName;


            try
            {
                _slowFitContext.TypeTrainigs.Update(existingType);


                _slowFitContext.SaveChanges();

                return Ok("Training type updated succesfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update type: {typeT.TypeName}");
            }
        }

        // DELETE api/<TyepeTrainingController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var typeT = _slowFitContext.TypeTrainigs.Where(u => u.TypeId == id).FirstOrDefault();

            if (typeT == null)
            {
                return NotFound(new { message = "Type not found" });
            }

            try
            {
                _slowFitContext.TypeTrainigs.Remove(typeT);
                _slowFitContext.SaveChanges();
                return Ok("Type delete succesfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error to delete plan {typeT.TypeName} ");
            }
        }
    }
}
