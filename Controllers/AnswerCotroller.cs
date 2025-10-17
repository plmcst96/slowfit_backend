using System;
using Microsoft.AspNetCore.Mvc;
using slowfit.DBModels;
using slowfit.DTORequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace slowfit.Controllers
{
    [Route("slowFit/answer")]
    [ApiController]
    public class AnswerCotroller(SlowFitContext slowFitCtx) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitCtx;

        [HttpGet]
        public ActionResult<IEnumerable<AnswerRes>> GetAnswers()
        {
            var answerList = new List<AnswerRes>();
            try
            {

                answerList = [.. _slowFitContext.Answers.Select(p => new AnswerRes
                {
                    AnswerId = p.AnswerId,
                    AnswerString = p.AnswerString,
                    QuestionId = p.QuestionId,
                })];

                if (answerList.Count == 0) return NoContent();

                return Ok(answerList);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<AnswerRes> GetAnswerById(int id)
        {
            try
            {
                var answer = _slowFitContext.Answers.Where(a => a.AnswerId == id).FirstOrDefault();
                if (answer == null) return NotFound();
                return Ok(answer);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
            
        }

        [HttpGet("byQuestion/{questionId}")]
        public ActionResult<IEnumerable<AnswerRes>> GetAnswersByQuestionId(int questionId)
        {
            try
            {
                var filteredAnswers = _slowFitContext.Answers.Where(a => a.QuestionId == questionId).ToList();
                if (filteredAnswers.Count == 0) return NotFound($"No answer found for user {questionId}.");
                return Ok(filteredAnswers);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
            
        }

        [HttpPost]
        public IActionResult CreateAnswer([FromBody] AnswerRes answer)
        {
            if (answer == null)
            {
                return BadRequest("Data amswet incorrect");
            }

            if (string.IsNullOrEmpty(answer.AnswerString) || answer.QuestionId == 0)
            {
                return BadRequest();
            }
            try
            {
                var ans = new Answer()
                {
                    AnswerString = answer.AnswerString,
                    QuestionId = answer.QuestionId
                };
                _slowFitContext.Answers.Add(ans);
                _slowFitContext.SaveChanges();
                return Ok("Answer created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create answer");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAnswer(int id, [FromBody] AnswerRes updatedAnswer)
        {
            var answer = _slowFitContext.Answers.Where(a => a.AnswerId == id).FirstOrDefault();
            if (answer == null) return NotFound();

            answer.AnswerString = updatedAnswer.AnswerString;

            try
            {
                _slowFitContext.Answers.Update(answer);


                _slowFitContext.SaveChanges();

                return Ok("Answer updated succesfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update answer: {updatedAnswer.AnswerId}");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAnswer(int id)
        {
            var answer = _slowFitContext.Answers.Where(a => a.AnswerId == id) .FirstOrDefault();
            if (answer == null) return NotFound(new { message = "Answer not found" });
            try
            {
                _slowFitContext.Answers.Remove(answer);
                _slowFitContext.SaveChanges();
                return Ok("Answer delete succesfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error to delete answer {id} ");
            }
        }
    }
}
