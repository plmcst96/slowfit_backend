using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using slowfit.DBModels;
using slowfit.DTORequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace slowfit.Controllers
{
    [Route("slowFit/response")]
    [ApiController]
    public class ResponseQuizController(SlowFitContext slowFitContext) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitContext;

        // GET: api/<ResponseQuizController>
        [HttpGet]
        public ActionResult<IEnumerable<ResponseQuizRes>> GetAll()
        {
            var responseList = new List<ResponseQuizRes>();
            try
            {

                responseList = _slowFitContext.ResponseQuizzes.Select(t => new ResponseQuizRes
                {
                    UserId = t.UserId,
                    ResponseId = t.ResponseId,
                    AnswerId = t.AnswerId,

                }).ToList();


                if (responseList.Count == 0) return NoContent();

                return Ok(responseList);
            }
            catch (Exception)
            {
                return BadRequest($"An error occurred");
            };
        }

        // GET api/<ResponseQuizController>/5
        [HttpGet("{id}")]
        public ActionResult<ResponseQuizRes> GetSingleResponse(int id)
        {
            try
            {
                var response = _slowFitContext.ResponseQuizzes.Where(t => t.ResponseId == id).FirstOrDefault();
                if (response == null) return NotFound();
                return Ok(response);
            }
            catch (Exception)
            {
                return BadRequest($"No response found with {id}");
            }
        }


        [HttpGet("byUser/{userId}")]
        public ActionResult<ResponseQuizRes> GetResponseQuizByUserId(int userId)
        {
            try
            {
                var response = _slowFitContext.ResponseQuizzes.Where(a => a.UserId == userId).ToList();
                if (response == null) return NotFound();
                return Ok(response);
            }
            catch (Exception)
            {
                return BadRequest($"No responses quiz found for {userId}");
            }
        }

        // POST api/<ResponseQuizController>
        [HttpPost]
        public IActionResult Post([FromBody] ResponseQuizRes response)
        {
            if (response == null)
            {
                return BadRequest("Invalid request body.");
            }

            if (response.UserId <= 0 ||
                response.AnswerId <= 0 )
            {
                return BadRequest();
            }

            try
            {
                var res = new ResponseQuiz()
                {
                    
                    UserId = response.UserId,
                    AnswerId = response.AnswerId,
                };
                _slowFitContext.ResponseQuizzes.Add(res);
                _slowFitContext.SaveChanges();
                return Ok("ResponseQuiz created successfully.");
            }
            catch (Exception)
            {
                return BadRequest($"Failed to create response");
            }
        }

        // PUT api/<ResponseQuizController>/5
        [HttpPut("{id}")]
        public IActionResult UpdateResponse(int id, [FromBody] ResponseQuizRes updateRes)
        {
            var response = _slowFitContext.ResponseQuizzes.Where(t => t.ResponseId == id).FirstOrDefault();
            if (response == null) return NotFound();

            response.UserId = updateRes.UserId;
            response.AnswerId = updateRes.AnswerId;
            try
            {
                _slowFitContext.ResponseQuizzes.Update(response);


                _slowFitContext.SaveChanges();

                return Ok("ResponseQuiz updated succesfully");
            }
            catch (Exception)
            {
                return BadRequest($"Failed to update response: {updateRes.ResponseId}");
            }
        }

        // DELETE api/<ResponseQuizController>/5
        [HttpDelete("{id}")]
        public IActionResult DeleteResponse(int id)
        {
            var response = _slowFitContext.ResponseQuizzes.Where(t => t.ResponseId == id).FirstOrDefault();
            if (response == null) return NotFound();
            try
            {
                _slowFitContext.ResponseQuizzes.Remove(response);
                _slowFitContext.SaveChanges();
                return Ok("ResponseQuiz deleted successfully.");
            }
            catch (Exception)
            {
                return BadRequest($"Failed to delete response: {id}");
            }
        }
    }
}
