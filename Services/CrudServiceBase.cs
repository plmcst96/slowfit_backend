using Microsoft.EntityFrameworkCore;

namespace slowfit.Services;

public abstract class CrudServiceBase<TEntity, TDto>(DbContext dbContext) : ICrudService<TDto>
    where TEntity : class
{
    protected readonly DbContext DbContext = dbContext;

    protected abstract DbSet<TEntity> Set { get; }

    protected abstract string EntityCode { get; }

    protected abstract string EntityName { get; }

    protected abstract int GetId(TEntity entity);

    protected abstract int GetDtoId(TDto dto);

    protected abstract TDto ToDto(TEntity entity);

    protected abstract TEntity CreateEntity(TDto dto);

    protected abstract void UpdateEntity(TEntity entity, TDto dto);
    protected abstract bool IsValid(TDto dto);

    public async Task<ServiceResult<IReadOnlyList<TDto>>> GetAllAsync()
    {
        var items = await Set.AsNoTracking().ToListAsync();
        var dtoList = items.Select(ToDto).ToList();

        return dtoList.Count == 0
            ? ServiceResult<IReadOnlyList<TDto>>.NoContent()
            : ServiceResult<IReadOnlyList<TDto>>.Ok(dtoList);
    }

    public async Task<ServiceResult<TDto>> GetByIdAsync(int id)
    {
        var entity = await FindByIdAsync(id);
        return entity == null
            ? ServiceResult<TDto>.NotFound($"{EntityCode}_not_found", $"{EntityName} not found.")
            : ServiceResult<TDto>.Ok(ToDto(entity));
    }

    public async Task<ServiceResult<TDto>> CreateAsync(TDto request)
    {
        if (!IsValid(request))
        {
            return ServiceResult<TDto>.BadRequest($"invalid_{EntityCode}", $"Invalid {EntityName} data.");
        }

        var entity = CreateEntity(request);
        Set.Add(entity);
        await DbContext.SaveChangesAsync();

        return ServiceResult<TDto>.Created(ToDto(entity));
    }

    public async Task<ServiceResult<TDto>> UpdateAsync(int id, TDto request)
    {
        if (!IsValid(request) || GetDtoId(request) != id)
        {
            return ServiceResult<TDto>.BadRequest($"invalid_{EntityCode}", "Invalid data or ID does not match.");
        }

        var entity = await FindByIdAsync(id);
        if (entity == null)
        {
            return ServiceResult<TDto>.NotFound($"{EntityCode}_not_found", $"{EntityName} not found.");
        }

        UpdateEntity(entity, request);
        await DbContext.SaveChangesAsync();

        return ServiceResult<TDto>.Ok(ToDto(entity));
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id)
    {
        var entity = await FindByIdAsync(id);
        if (entity == null)
        {
            return ServiceResult<object>.NotFound($"{EntityCode}_not_found", $"{EntityName} not found.");
        }

        Set.Remove(entity);
        await DbContext.SaveChangesAsync();

        return ServiceResult<object>.NoContent();
    }

    private async Task<TEntity?> FindByIdAsync(int id)
    {
        var items = await Set.ToListAsync();
        return items.FirstOrDefault(entity => GetId(entity) == id);
    }
}
