using Microsoft.EntityFrameworkCore;
using slowfit.DBModels;
using slowfit.DTORequest;

namespace slowfit.Services;

public sealed class QuestionService(SlowFitContext context) : CrudServiceBase<Question, QuestionRes>(context)
{
    protected override DbSet<Question> Set => ((SlowFitContext)DbContext).Questions;
    protected override string EntityCode => "question";
    protected override string EntityName => "Question";
    protected override int GetId(Question entity) => entity.QuestionId;
    protected override int GetDtoId(QuestionRes dto) => dto.QuestionId;
    protected override QuestionRes ToDto(Question entity) => new() { QuestionId = entity.QuestionId, QuestionString = entity.QuestionString };
    protected override Question CreateEntity(QuestionRes dto) => new() { QuestionString = dto.QuestionString.Trim() };
    protected override void UpdateEntity(Question entity, QuestionRes dto) => entity.QuestionString = dto.QuestionString.Trim();
    protected override bool IsValid(QuestionRes dto) => dto != null && !string.IsNullOrWhiteSpace(dto.QuestionString);
}

public interface IAnswerService : ICrudService<AnswerRes>
{
    Task<ServiceResult<IReadOnlyList<AnswerRes>>> GetByQuestionAsync(int questionId);
}

public sealed class AnswerService(SlowFitContext context) : CrudServiceBase<Answer, AnswerRes>(context), IAnswerService
{
    protected override DbSet<Answer> Set => ((SlowFitContext)DbContext).Answers;
    protected override string EntityCode => "answer";
    protected override string EntityName => "Answer";
    protected override int GetId(Answer entity) => entity.AnswerId;
    protected override int GetDtoId(AnswerRes dto) => dto.AnswerId;
    protected override AnswerRes ToDto(Answer entity) => new() { AnswerId = entity.AnswerId, AnswerString = entity.AnswerString, QuestionId = entity.QuestionId };
    protected override Answer CreateEntity(AnswerRes dto) => new() { AnswerString = dto.AnswerString.Trim(), QuestionId = dto.QuestionId };
    protected override void UpdateEntity(Answer entity, AnswerRes dto)
    {
        entity.AnswerString = dto.AnswerString.Trim();
        entity.QuestionId = dto.QuestionId;
    }
    protected override bool IsValid(AnswerRes dto) => dto != null && !string.IsNullOrWhiteSpace(dto.AnswerString) && dto.QuestionId > 0;

    public async Task<ServiceResult<IReadOnlyList<AnswerRes>>> GetByQuestionAsync(int questionId)
    {
        var entities = await Set.AsNoTracking()
            .Where(a => a.QuestionId == questionId)
            .ToListAsync();
        var answers = entities.Select(ToDto).ToList();

        return ServiceResult<IReadOnlyList<AnswerRes>>.Ok(answers);
    }
}

public interface IResponseQuizService : ICrudService<ResponseQuizRes>
{
    Task<ServiceResult<IReadOnlyList<ResponseQuizRes>>> GetByUserAsync(int userId);
    Task<ServiceResult<IReadOnlyList<ResponseQuizRes>>> CreateManyAsync(IReadOnlyList<ResponseQuizRes> requests);
}

public sealed class ResponseQuizService(SlowFitContext context) : CrudServiceBase<ResponseQuiz, ResponseQuizRes>(context), IResponseQuizService
{
    protected override DbSet<ResponseQuiz> Set => ((SlowFitContext)DbContext).ResponseQuizzes;
    protected override string EntityCode => "response_quiz";
    protected override string EntityName => "Response quiz";
    protected override int GetId(ResponseQuiz entity) => entity.ResponseId;
    protected override int GetDtoId(ResponseQuizRes dto) => dto.ResponseId;
    protected override ResponseQuizRes ToDto(ResponseQuiz entity) => new()
    {
        ResponseId = entity.ResponseId,
        AnswerId = entity.AnswerId,
        UserId = entity.UserId,
        AnswerString = entity.AnswerString
    };
    protected override ResponseQuiz CreateEntity(ResponseQuizRes dto) => new()
    {
        AnswerId = dto.AnswerId,
        UserId = dto.UserId,
        AnswerString = dto.AnswerString
    };
    protected override void UpdateEntity(ResponseQuiz entity, ResponseQuizRes dto)
    {
        entity.AnswerId = dto.AnswerId;
        entity.UserId = dto.UserId;
        entity.AnswerString = dto.AnswerString;
    }
    protected override bool IsValid(ResponseQuizRes dto) => dto != null && dto.AnswerId > 0 && dto.UserId > 0;

    public async Task<ServiceResult<IReadOnlyList<ResponseQuizRes>>> GetByUserAsync(int userId)
    {
        var entities = await Set.AsNoTracking()
            .Where(r => r.UserId == userId)
            .ToListAsync();
        var responses = entities.Select(ToDto).ToList();

        return ServiceResult<IReadOnlyList<ResponseQuizRes>>.Ok(responses);
    }

    public async Task<ServiceResult<IReadOnlyList<ResponseQuizRes>>> CreateManyAsync(IReadOnlyList<ResponseQuizRes> requests)
    {
        if (requests == null || requests.Count == 0 || requests.Any(request => !IsValid(request)))
        {
            return ServiceResult<IReadOnlyList<ResponseQuizRes>>.BadRequest("invalid_response_quiz", "Invalid response quiz data.");
        }

        var entities = requests.Select(CreateEntity).ToList();
        Set.AddRange(entities);
        await DbContext.SaveChangesAsync();

        return ServiceResult<IReadOnlyList<ResponseQuizRes>>.Created(entities.Select(ToDto).ToList());
    }
}
