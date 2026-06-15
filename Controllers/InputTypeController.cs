using Microsoft.AspNetCore.Mvc;
using slowfit.DBModels;
using slowfit.DTORequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace slowfit.Controllers
{
    [Route("slowFit/input")]
    [ApiController]
    public class InputTypeController(SlowFitContext slowFitCtx) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitCtx;

        [HttpGet]
        public ActionResult<IEnumerable<InputTypeRes>> GetInputTypes()
        {
            var inputList = new List<InputTypeRes>();
            try
            {
                inputList = _slowFitContext.InputTypes.Select(p => new InputTypeRes
                {
                    InputTypeId = p.InputTypeId,
                    InputTypeName = p.InputTypeName,
                    
                }).ToList();

                if (inputList.Count == 0) return NoContent();

                return Ok(inputList);
            }
            catch (Exception)
            {
                return BadRequest($"An error occurred");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<InputTypeRes> GetInputTypeById(int id)
        {
            try
            {
                var inputType = _slowFitContext.InputTypes.Where(it => it.InputTypeId == id).FirstOrDefault();
                if (inputType == null) return NotFound();
                return Ok(inputType);
            }
            catch (Exception)
            {
                return BadRequest( $"An error occurred");
            }


        }

        [HttpPost]
        public ActionResult<InputTypeRes> CreateInputType(InputTypeRes inputType)
        {
            if (inputType == null)
            {
                return BadRequest("Invalid request body.");
            }

            if (string.IsNullOrEmpty(inputType.InputTypeName) || inputType.InputTypeId == 0)
            {
                return BadRequest();
            }

            try
            {
                var input = new InputType() {
                    InputTypeName = inputType.InputTypeName
                };
                _slowFitContext.InputTypes.Add(input);
                _slowFitContext.SaveChanges();
                return Ok("Input created successfully.");
            }
            catch (Exception)
            {
                return BadRequest($"Failed to create Input");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateInputType(int id, InputTypeRes updatedInputType)
        {
            var inputType = _slowFitContext.InputTypes.Where(it => it.InputTypeId == id).FirstOrDefault();
            if (inputType == null) return NotFound();

            inputType.InputTypeName = updatedInputType.InputTypeName;
            try
            {
                _slowFitContext.InputTypes.Update(inputType);


                _slowFitContext.SaveChanges();

                return NoContent();
            }
            catch (Exception)
            {
                return BadRequest($"Failed to update input: {updatedInputType.InputTypeName}");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteInputType(int id)
        {
            var input = _slowFitContext.InputTypes.Where(t => t.InputTypeId == id).FirstOrDefault();
            if (input == null) return NotFound();
            try
            {
                _slowFitContext.InputTypes.Remove(input);
                _slowFitContext.SaveChanges();
                return NoContent();
            }
            catch (Exception)
            {
                return BadRequest($"Error to delete input {input.InputTypeName} ");
            }
        }
    }
}
