using Microsoft.EntityFrameworkCore;
using slowfit.DBModels;
using slowfit.DTORequest;
using slowfit.DTOResponse;

namespace slowfit.Services;

public interface IQuizService
{
    Task<ServiceResult<IReadOnlyList<QuizUserResponse>>> GetAllAsync(string? type);
    Task<ServiceResult<QuizUserResponse>> GetByIdAsync(int id);
    Task<ServiceResult<QuizUserRes>> CreateAsync(QuizUserRes request);
    Task<ServiceResult<QuizUserRes>> UpdateAsync(int id, QuizUserRes request);
    Task<ServiceResult<object>> DeleteAsync(int id);
}

public sealed class QuizService(SlowFitContext context) : IQuizService
{
    private readonly SlowFitContext _context = context;

    public async Task<ServiceResult<IReadOnlyList<QuizUserResponse>>> GetAllAsync(string? type)
    {
        var query = _context.Quizzes.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(type)) query = query.Where(q => q.Type == type);

        var quizzes = await ProjectToResponse(query.OrderBy(q => q.Input).ThenBy(q => q.QuizId)).ToListAsync();
        return ServiceResult<IReadOnlyList<QuizUserResponse>>.Ok(quizzes);
    }

    public async Task<ServiceResult<QuizUserResponse>> GetByIdAsync(int id)
    {
        var quiz = await ProjectToResponse(_context.Quizzes.AsNoTracking().Where(q => q.QuizId == id)).FirstOrDefaultAsync();
        return quiz == null ? ServiceResult<QuizUserResponse>.Ok(default!) : ServiceResult<QuizUserResponse>.Ok(quiz);
    }

    public async Task<ServiceResult<QuizUserRes>> CreateAsync(QuizUserRes request)
    {
        if (!IsValid(request)) return ServiceResult<QuizUserRes>.BadRequest("invalid_quiz", "I dati del quiz non sono validi.");

        var quiz = new Quiz { QuestionId = request.QuestionId, InputTypeId = request.InputTypeId, Input = request.Input, SingleResponse = request.SingleResponse, Type = request.Type };
        _context.Quizzes.Add(quiz);
        await _context.SaveChangesAsync();

        request.QuizId = quiz.QuizId;
        return ServiceResult<QuizUserRes>.Created(request);
    }

    public async Task<ServiceResult<QuizUserRes>> UpdateAsync(int id, QuizUserRes request)
    {
        if (!IsValid(request)) return ServiceResult<QuizUserRes>.BadRequest("invalid_quiz", "I dati del quiz non sono validi.");

        var quiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.QuizId == id);
        if (quiz == null) return ServiceResult<QuizUserRes>.NotFound("quiz_not_found", "Quiz non trovato.");

        quiz.QuestionId = request.QuestionId;
        quiz.InputTypeId = request.InputTypeId;
        quiz.Input = request.Input;
        quiz.SingleResponse = request.SingleResponse;
        quiz.Type = request.Type;
        await _context.SaveChangesAsync();

        request.QuizId = id;
        return ServiceResult<QuizUserRes>.Ok(request);
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id)
    {
        var quiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.QuizId == id);
        if (quiz == null) return ServiceResult<object>.NotFound("quiz_not_found", "Quiz non trovato.");

        _context.Quizzes.Remove(quiz);
        await _context.SaveChangesAsync();
        return ServiceResult<object>.NoContent();
    }

    private static bool IsValid(QuizUserRes request) => request != null && request.QuestionId > 0 && request.InputTypeId > 0;

    private IQueryable<QuizUserResponse> ProjectToResponse(IQueryable<Quiz> query) => query.Select(quiz => new QuizUserResponse
    {
        QuizId = quiz.QuizId,
        QuestionId = quiz.QuestionId,
        Input = quiz.Input,
        InputTypeId = quiz.InputTypeId,
        SingleResponse = quiz.SingleResponse,
        Type = quiz.Type,
        QuestionText = _context.Questions.Where(q => q.QuestionId == quiz.QuestionId).Select(q => q.QuestionString).FirstOrDefault(),
        Answers = _context.Answers.Where(a => a.QuestionId == quiz.QuestionId).Select(a => new AnswerRes { AnswerId = a.AnswerId, AnswerString = a.AnswerString, QuestionId = a.QuestionId }).ToList()
    });
}
