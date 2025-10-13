using System;
using Microsoft.AspNetCore.Mvc;
using slowfit.DBModels;
using slowfit.DTORequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace slowfit.Controllers
{
    [Route("slowFit/question")]
    [ApiController]
    public class QuestionController(SlowFitContext slowFitCtx) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitCtx;

        [HttpGet]
        public ActionResult<IEnumerable<QuestionRes>> GetQuestions()
        {
            var questionList = new List<QuestionRes>();
            try
            {

                questionList = _slowFitContext.Questions.Select(p => new QuestionRes
                {
                    QuestionId = p.QuestionId,
                    QuestionString = p.QuestionString,
                   
                }).ToList();

                if (questionList.Count == 0) return NoContent();

                return Ok(questionList);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<QuestionRes> GetQuestionById(int id)
        {
            try
            {
                var question = _slowFitContext.Questions.Where(q => q.QuestionId == id).FirstOrDefault();
                if (question == null) return NotFound();
                return Ok(question);
            }
            catch (Exception ex)
            {
                return BadRequest($"No question found with {id}");
            }
            
        }

        [HttpPost]
        public ActionResult<QuestionRes> CreateQuestion([FromBody] QuestionRes question)
        {
            if (question == null)
            {
                return BadRequest("Data plan incorrect");
            }

            if (string.IsNullOrEmpty(question.QuestionString))
            {
                return BadRequest();
            }
            try
            {
                var ques = new Question()
                {
                    QuestionString = question.QuestionString
                };
                _slowFitContext.Questions.Add(ques);
                _slowFitContext.SaveChanges();
                return Ok("Question created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create question");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateQuestion(int id, [FromBody] QuestionRes updatedQuestion)
        {
            var question = _slowFitContext.Questions.Where(q => q.QuestionId == id).FirstOrDefault();
            if (question == null) return NotFound();

            question.QuestionString = updatedQuestion.QuestionString;

            try
            {
                _slowFitContext.Questions.Update(question);


                _slowFitContext.SaveChanges();

                return Ok("Question updated succesfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update question: {updatedQuestion.QuestionId}");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteQuestion(int id)
        {
            var question = _slowFitContext.Questions.Where(q => q.QuestionId == id).FirstOrDefault();
            if (question == null) return NotFound();
            try
            {
                _slowFitContext.Questions.Remove(question);
                _slowFitContext.SaveChanges();
                return Ok("Question delete succesfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error to delete question {id} ");
            }
        }
    }
}
