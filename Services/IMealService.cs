using slowfit.DTORequest;

namespace slowfit.Services;

public interface IMealService
{
    Task<ServiceResult<IReadOnlyList<MealRes>>> GetAllAsync();
    Task<ServiceResult<MealRes>> GetByIdAsync(int id);
    Task<ServiceResult<MealRes>> GetMealWithIngredientsAsync(int id);
    Task<ServiceResult<IReadOnlyList<MealRes>>> GetByIngredientAsync(int ingredientId);
    Task<ServiceResult<IReadOnlyList<MealRes>>> GetByCategoryAsync(int categoryId);
    Task<ServiceResult<IReadOnlyList<MealRes>>> GetByDayAsync(int dayId);
    Task<ServiceResult<object>> CreateWithIngredientsAsync(MealResWithIngredients request);
    Task<ServiceResult<object>> UpdateWithIngredientsAsync(int id, MealResWithIngredients request);
    Task<ServiceResult<object>> DeleteAsync(int id);
}
