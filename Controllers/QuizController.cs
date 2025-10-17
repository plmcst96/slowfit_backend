using System;
using Microsoft.AspNetCore.Mvc;
using slowfit.DBModels;
using slowfit.DTORequest;
using slowfit.DTOResponse;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace slowfit.Controllers
{
    [Route("slowFit/quiz")]
    [ApiController]
    public class QuizController(SlowFitContext slowFitCtx) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitCtx;

        // GET: slowFit/quiz?type=...
        [HttpGet]
        public ActionResult<IEnumerable<QuizUserRes>> GetQuizzes([FromQuery] string? type)
        {
            try
            {
                var query = _slowFitContext.Quizzes.AsQueryable();

                // 🔹 Filtro per Type (QueryString)
                if (!string.IsNullOrEmpty(type))
                {
                    query = query.Where(q => q.Type == type);
                }

                // 🧮 Ordina: prima quelli senza input
                query = query.OrderBy(q => q.Input)  // false (0) prima di true (1)
                             .ThenBy(q => q.QuizId);

                var quizList = query
                    .Select(q => new QuizUserResponse
                    {
                        QuizId = q.QuizId,
                        QuestionId = q.QuestionId,
                        Input = q.Input,
                        InputTypeId = q.InputTypeId,
                        SingleResponse = q.SingleResponse,
                        Type = q.Type,
                        // 🔹 Recupero testo della domanda
                        QuestionText = _slowFitContext.Questions
                                    .Where(ques => ques.QuestionId == q.QuestionId)
                                    .Select(ques => ques.QuestionString)
                                    .FirstOrDefault(),
                        // 🔽 Carico tutte le Answers legate al QuestionId
                        Answers = _slowFitContext.Answers
                                    .Where(a => a.QuestionId == q.QuestionId)
                                    .Select(a => new AnswerRes
                                    {
                                        AnswerId = a.AnswerId,
                                        AnswerString = a.AnswerString,
                                    }).ToList()
                    }).ToList();

                if (quizList.Count == 0) return NoContent();

                return Ok(quizList);
            }
            catch (Exception)
            {
                return BadRequest("An error occurred");
            }
        }

        // GET: api/Quiz/5
        [HttpGet("{id}")]
        public ActionResult<QuizUserRes> GetQuiz(int id)
        {
            try
            {
                var quiz = _slowFitContext.Quizzes.Where(t => t.QuizId == id).FirstOrDefault();
                if (quiz == null) return NotFound();
                return Ok(quiz);
            }
            catch (Exception ex)
            {
                return BadRequest($"No quiz found with {id}");
            }
        }


        // POST: api/Quiz
        [HttpPost]
        public ActionResult<QuizUserRes> CreateQuiz([FromBody] QuizUserRes quizDto)
        {

            if (quizDto == null)
            {
                return BadRequest("Invalid request body.");
            }

            if (quizDto.QuestionId <= 0 || quizDto.InputTypeId <= 0)
            {
                return BadRequest();
            }


            try
            {
                var qz = new Quiz()
                {
                    QuestionId = quizDto.QuestionId,
                    InputTypeId = quizDto.InputTypeId,
                    Input = quizDto.Input,
                    SingleResponse = quizDto.SingleResponse
                    
                };
                _slowFitContext.Quizzes.Add(qz);
                _slowFitContext.SaveChanges();
                return Ok("Quiz created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create Quiz");
            }
        }

        // PUT: api/Quiz/5
        [HttpPut("{id}")]
        public IActionResult UpdateQuiz(int id, [FromBody] QuizUserRes quizDto)
        {
            var quiz = _slowFitContext.Quizzes.Where(q => q.QuizId == id).FirstOrDefault();
            if (quiz == null)
            {
                return NotFound();
            }

            quiz.QuestionId = quizDto.QuestionId;
            quiz.InputTypeId = quizDto.InputTypeId;
            quiz.Input = quizDto.Input;
            quiz.SingleResponse = quizDto.SingleResponse;

            try
            {
                _slowFitContext.Quizzes.Update(quiz);


                _slowFitContext.SaveChanges();

                return Ok("Quiz updated succesfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update quiz: {quiz.QuizId}");
            }
        }

        // DELETE: api/Quiz/5
        [HttpDelete("{id}")]
        public IActionResult DeleteQuiz(int id)
        {
            var quiz = _slowFitContext.Quizzes.Where(q => q.QuizId == id).FirstOrDefault();
            if (quiz == null)
            {
                return NotFound();
            }

            try
            {
                _slowFitContext.Quizzes.Remove(quiz);
                _slowFitContext.SaveChanges();

                return Ok($"Quiz has been successfully cancelled");
            }
            catch (Exception ex) 
            { 
                return BadRequest($"No quiz found whit {id}"); 
            }
        }
    }
}
