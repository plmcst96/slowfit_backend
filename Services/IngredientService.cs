using Microsoft.EntityFrameworkCore;
using slowfit.DBModels;
using slowfit.DTORequest;

namespace slowfit.Services;

public sealed class IngredientService(SlowFitContext context) : CrudServiceBase<Ingredient, IngredientRes>(context), IIngredientService
{
    protected override DbSet<Ingredient> Set => ((SlowFitContext)DbContext).Ingredients;
    protected override string EntityCode => "ingredient";
    protected override string EntityName => "Ingredient";
    protected override int GetId(Ingredient entity) => entity.IngredientId;
    protected override int GetDtoId(IngredientRes dto) => dto.IngredientId;
    protected override IngredientRes ToDto(Ingredient entity) => new()
    {
        IngredientId = entity.IngredientId,
        Name = entity.Name,
        Calories = entity.Calories,
        Protein = entity.Protein,
        Fats = entity.Fats,
        Carbohydrate = entity.Carbohydrate
    };
    protected override Ingredient CreateEntity(IngredientRes dto) => new()
    {
        Name = dto.Name.Trim(),
        Calories = dto.Calories,
        Protein = dto.Protein,
        Fats = dto.Fats,
        Carbohydrate = dto.Carbohydrate
    };
    protected override void UpdateEntity(Ingredient entity, IngredientRes dto)
    {
        entity.Name = dto.Name.Trim();
        entity.Calories = dto.Calories;
        entity.Protein = dto.Protein;
        entity.Fats = dto.Fats;
        entity.Carbohydrate = dto.Carbohydrate;
    }
    protected override bool IsValid(IngredientRes dto) => dto != null && !string.IsNullOrWhiteSpace(dto.Name);

    public async Task<ServiceResult<IReadOnlyList<IngredientRes>>> SearchAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ServiceResult<IReadOnlyList<IngredientRes>>.BadRequest("invalid_search", "The name parameter cannot be empty.");
        }

        var normalizedName = name.Trim().ToLower();
        var ingredients = await Set.AsNoTracking()
            .Where(i => i.Name.ToLower().Contains(normalizedName))
            .Select(i => ToDto(i))
            .ToListAsync();

        return ingredients.Count == 0
            ? ServiceResult<IReadOnlyList<IngredientRes>>.NotFound("ingredient_not_found", "No ingredients found.")
            : ServiceResult<IReadOnlyList<IngredientRes>>.Ok(ingredients);
    }
}
