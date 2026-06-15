namespace slowfit.Services;

public interface ICrudService<TDto>
{
    Task<ServiceResult<IReadOnlyList<TDto>>> GetAllAsync();

    Task<ServiceResult<TDto>> GetByIdAsync(int id);

    Task<ServiceResult<TDto>> CreateAsync(TDto request);


    Task<ServiceResult<TDto>> UpdateAsync(int id, TDto request);

    Task<ServiceResult<object>> DeleteAsync(int id);
}
