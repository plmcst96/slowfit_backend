using Microsoft.EntityFrameworkCore;
using slowfit.DBModels;
using slowfit.DTORequest;

namespace slowfit.Services;

public sealed class MealService(SlowFitContext context) : IMealService
{
    private readonly SlowFitContext _context = context;

    public async Task<ServiceResult<IReadOnlyList<MealRes>>> GetAllAsync()
    {
        var meals = await _context.Meals
            .AsNoTracking()
            .Select(m => new MealRes
            {
                MealId = m.MealId,
                Name = m.Name,
                Description = m.Description,
                Recipe = m.Recipe,
                Calories = m.Calories,
                PreparingTime = m.PreparingTime,
                Protein = m.Protein,
                Fats = m.Fats,
                Carbohydrate = m.Carbohydrate,
                ImageMeal = m.ImageMeal,
                Difficulty = m.Difficulty,
                CategoryId = m.CategoryId
            })
            .ToListAsync();

        return ServiceResult<IReadOnlyList<MealRes>>.Ok(meals);
    }

    public async Task<ServiceResult<MealRes>> GetByIdAsync(int id)
    {
        var meal = await _context.Meals
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.MealId == id);

        if (meal == null)
            return ServiceResult<MealRes>.Ok(default!);

        return ServiceResult<MealRes>.Ok(ToRes(meal));
    }

    public async Task<ServiceResult<MealRes>> GetMealWithIngredientsAsync(int id)
    {
        var meal = await _context.Meals
            .AsNoTracking()
            .Include(m => m.MealIngredients)
                .ThenInclude(mi => mi.Ingredient)
            .FirstOrDefaultAsync(m => m.MealId == id);

        if (meal == null)
            return ServiceResult<MealRes>.Ok(default!);

        return ServiceResult<MealRes>.Ok(ToResWithIngredients(meal));
    }

    public async Task<ServiceResult<IReadOnlyList<MealRes>>> GetByIngredientAsync(int ingredientId)
    {
        var meals = await _context.MealIngredients
            .AsNoTracking()
            .Where(mi => mi.IngredientId == ingredientId)
            .Include(mi => mi.Meal)
                .ThenInclude(m => m.MealIngredients)
                .ThenInclude(mi => mi.Ingredient)
            .Select(mi => new MealRes
            {
                MealId = mi.Meal.MealId,
                Name = mi.Meal.Name,
                Description = mi.Meal.Description,
                Recipe = mi.Meal.Recipe,
                Calories = mi.Meal.Calories,
                Protein = mi.Meal.Protein,
                Fats = mi.Meal.Fats,
                Carbohydrate = mi.Meal.Carbohydrate,
                PreparingTime = mi.Meal.PreparingTime,
                ImageMeal = mi.Meal.ImageMeal,
                Difficulty = mi.Meal.Difficulty,
                CategoryId = mi.Meal.CategoryId,
                Ingredients = mi.Meal.MealIngredients.Select(m => new IngredientRes
                {
                    IngredientId = m.Ingredient.IngredientId,
                    Name = m.Ingredient.Name,
                    Quantity = m.Quantity,
                    Unit = m.Unit
                }).ToList()
            })
            .ToListAsync();

        return ServiceResult<IReadOnlyList<MealRes>>.Ok(meals);
    }

    public async Task<ServiceResult<IReadOnlyList<MealRes>>> GetByCategoryAsync(int categoryId)
    {
        var meals = await _context.Meals
            .AsNoTracking()
            .Where(m => m.CategoryId == categoryId)
            .Select(m => new MealRes
            {
                MealId = m.MealId,
                Name = m.Name,
                Description = m.Description,
                Recipe = m.Recipe,
                Calories = m.Calories,
                Protein = m.Protein,
                Fats = m.Fats,
                Carbohydrate = m.Carbohydrate,
                PreparingTime = m.PreparingTime,
                ImageMeal = m.ImageMeal,
                Difficulty = m.Difficulty,
                CategoryId = m.CategoryId
            })
            .ToListAsync();

        return ServiceResult<IReadOnlyList<MealRes>>.Ok(meals);
    }

    public async Task<ServiceResult<IReadOnlyList<MealRes>>> GetByDayAsync(int dayId)
    {
        var meals = await _context.NutritionMeals
            .AsNoTracking()
            .Where(nm => nm.DayId == dayId)
            .Include(nm => nm.Meal)
            .Select(nm => new MealRes
            {
                MealId = nm.Meal.MealId,
                Name = nm.Meal.Name,
                Description = nm.Meal.Description,
                Recipe = nm.Meal.Recipe,
                Calories = nm.Meal.Calories,
                Protein = nm.Meal.Protein,
                Fats = nm.Meal.Fats,
                Carbohydrate = nm.Meal.Carbohydrate,
                PreparingTime = nm.Meal.PreparingTime,
                ImageMeal = nm.Meal.ImageMeal,
                Difficulty = nm.Meal.Difficulty,
                CategoryId = nm.Meal.CategoryId,
                DayId = nm.DayId
            })
            .Distinct()
            .ToListAsync();

        return ServiceResult<IReadOnlyList<MealRes>>.Ok(meals);
    }

    public async Task<ServiceResult<object>> CreateWithIngredientsAsync(MealResWithIngredients request)
    {
        if (request == null)
            return ServiceResult<object>.BadRequest("invalid_meal", "I dati della ricetta sono obbligatori.");
        if (string.IsNullOrWhiteSpace(request.Name))
            return ServiceResult<object>.BadRequest("missing_meal_name", "Il nome della ricetta è obbligatorio.");
        if (request.Ingredients == null || request.Ingredients.Count == 0)
            return ServiceResult<object>.BadRequest("missing_ingredients", "Aggiungi almeno un ingrediente.");

        try
        {
            var meal = new Meal
            {
                Name = request.Name,
                Description = request.Description,
                Recipe = request.Recipe,
                Calories = request.Calories,
                Protein = request.Protein,
                Fats = request.Fats,
                Carbohydrate = request.Carbohydrate,
                PreparingTime = request.PreparingTime,
                ImageMeal = request.ImageMeal,
                Difficulty = request.Difficulty,
                CategoryId = request.CategoryId
            };

            _context.Meals.Add(meal);
            await _context.SaveChangesAsync();

            foreach (var ingredient in request.Ingredients)
            {
                if (ingredient.IngredientId <= 0)
                    return ServiceResult<object>.BadRequest("invalid_ingredient", "Uno o più ingredienti selezionati non sono validi.");
                if (ingredient.Quantity <= 0)
                    return ServiceResult<object>.BadRequest("invalid_ingredient_quantity", "La quantità degli ingredienti deve essere maggiore di zero.");

                _context.MealIngredients.Add(new MealIngredient
                {
                    MealId = meal.MealId,
                    IngredientId = ingredient.IngredientId,
                    Quantity = ingredient.Quantity,
                    Unit = ingredient.Unit ?? "g"
                });
            }

            await _context.SaveChangesAsync();

            return ServiceResult<object>.Ok(new { message = "Ricetta creata con successo.", mealId = meal.MealId });
        }
        catch (DbUpdateException)
        {
            return ServiceResult<object>.Error("meal_save_failed", "Non è stato possibile salvare la ricetta. Riprova.");
        }
    }

    public async Task<ServiceResult<object>> UpdateWithIngredientsAsync(int id, MealResWithIngredients request)
    {
        if (request == null)
            return ServiceResult<object>.BadRequest("invalid_meal", "I dati della ricetta sono obbligatori.");
        if (string.IsNullOrWhiteSpace(request.Name))
            return ServiceResult<object>.BadRequest("missing_meal_name", "Il nome della ricetta è obbligatorio.");
        if (request.Ingredients == null || request.Ingredients.Count == 0)
            return ServiceResult<object>.BadRequest("missing_ingredients", "Aggiungi almeno un ingrediente.");

        try
        {
            var meal = await _context.Meals.FirstOrDefaultAsync(m => m.MealId == id);
            if (meal == null)
                return ServiceResult<object>.NotFound("meal_not_found", "Ricetta non trovata.");

            meal.Name = request.Name;
            meal.Description = request.Description;
            meal.Recipe = request.Recipe;
            meal.Calories = request.Calories;
            meal.Protein = request.Protein;
            meal.Fats = request.Fats;
            meal.Carbohydrate = request.Carbohydrate;
            meal.PreparingTime = request.PreparingTime;
            meal.ImageMeal = request.ImageMeal;
            meal.Difficulty = request.Difficulty;
            meal.CategoryId = request.CategoryId;

            var existingIngredients = await _context.MealIngredients.Where(mi => mi.MealId == id).ToListAsync();
            _context.MealIngredients.RemoveRange(existingIngredients);

            foreach (var ingredient in request.Ingredients)
            {
                if (ingredient.IngredientId <= 0)
                    return ServiceResult<object>.BadRequest("invalid_ingredient", "Uno o più ingredienti selezionati non sono validi.");
                if (ingredient.Quantity <= 0)
                    return ServiceResult<object>.BadRequest("invalid_ingredient_quantity", "La quantità degli ingredienti deve essere maggiore di zero.");

                _context.MealIngredients.Add(new MealIngredient
                {
                    MealId = id,
                    IngredientId = ingredient.IngredientId,
                    Quantity = ingredient.Quantity,
                    Unit = ingredient.Unit ?? "g"
                });
            }

            await _context.SaveChangesAsync();

            return ServiceResult<object>.Ok(new { message = "Ricetta aggiornata con successo.", mealId = id });
        }
        catch (DbUpdateException)
        {
            return ServiceResult<object>.Error("meal_update_failed", "Non è stato possibile aggiornare la ricetta. Riprova.");
        }
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id)
    {
        if (id <= 0)
            return ServiceResult<object>.BadRequest("invalid_meal", "Ricetta non valida.");

        try
        {
            var meal = await _context.Meals.FirstOrDefaultAsync(m => m.MealId == id);
            if (meal == null)
                return ServiceResult<object>.NotFound("meal_not_found", "Ricetta non trovata.");

            var ingredients = await _context.MealIngredients.Where(mi => mi.MealId == id).ToListAsync();
            _context.MealIngredients.RemoveRange(ingredients);
            _context.Meals.Remove(meal);
            await _context.SaveChangesAsync();

            return ServiceResult<object>.Ok(new { message = "Ricetta e relativi ingredienti eliminati con successo.", mealId = id });
        }
        catch (DbUpdateException)
        {
            return ServiceResult<object>.Error("meal_delete_failed", "Non è stato possibile eliminare la ricetta. Riprova.");
        }
    }

    private static MealRes ToRes(Meal meal) => new()
    {
        MealId = meal.MealId,
        Name = meal.Name,
        Description = meal.Description,
        Recipe = meal.Recipe,
        Calories = meal.Calories,
        PreparingTime = meal.PreparingTime,
        Protein = meal.Protein,
        Fats = meal.Fats,
        Carbohydrate = meal.Carbohydrate,
        ImageMeal = meal.ImageMeal,
        Difficulty = meal.Difficulty,
        CategoryId = meal.CategoryId
    };

    private static MealRes ToResWithIngredients(Meal meal)
    {
        var res = ToRes(meal);
        res.Ingredients = meal.MealIngredients.Select(mi => new IngredientRes
        {
            IngredientId = mi.Ingredient.IngredientId,
            Name = mi.Ingredient.Name,
            Quantity = mi.Quantity,
            Unit = mi.Unit
        }).ToList();
        return res;
    }
}
