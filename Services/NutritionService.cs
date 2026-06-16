using Microsoft.EntityFrameworkCore;
using slowfit.DBModels;
using slowfit.DTORequest;

namespace slowfit.Services;

public sealed class NutritionService(SlowFitContext context) : INutritionService
{
    private readonly SlowFitContext _context = context;

    public async Task<ServiceResult<IReadOnlyList<NutritionRes>>> GetAllAsync()
    {
        var nutritions = await _context.Nutritions
            .AsNoTracking()
            .Where(n => n.ExpirationDate == null)
            .Include(n => n.NutritionMeals).ThenInclude(nm => nm.Meal)
            .ToListAsync();

        var result = nutritions.Select(ToRes).ToList();
        return ServiceResult<IReadOnlyList<NutritionRes>>.Ok(result);
    }

    public async Task<ServiceResult<NutritionRes>> GetByIdAsync(int id)
    {
        var nutrition = await _context.Nutritions
            .AsNoTracking()
            .Include(n => n.NutritionMeals).ThenInclude(nm => nm.Meal)
            .FirstOrDefaultAsync(n => n.NutritionId == id && n.ExpirationDate == null);

        if (nutrition == null)
            return ServiceResult<NutritionRes>.Ok(default!);

        return ServiceResult<NutritionRes>.Ok(ToRes(nutrition));
    }

    public async Task<ServiceResult<IReadOnlyList<NutritionRes>>> GetByUserAsync(int userId)
    {
        var nutritions = await _context.Nutritions
            .AsNoTracking()
            .Include(n => n.NutritionMeals).ThenInclude(nm => nm.Meal)
            .Where(n => n.UserId == userId && n.ExpirationDate == null)
            .ToListAsync();

        var result = nutritions.Select(ToRes).ToList();
        return ServiceResult<IReadOnlyList<NutritionRes>>.Ok(result);
    }

    public async Task<ServiceResult<object>> CreateAsync(NutritionResPost request)
    {
        // Controllo base: request null
        if (request == null)
            return ServiceResult<object>.BadRequest("invalid_nutrition", "I dati del piano nutrizionale sono obbligatori.");

        // Controllo proprietà principali
        if (request.UserId <= 0)
            return ServiceResult<object>.BadRequest("invalid_user", "Utente non valido.");

        if (request.TypeNutritionId <= 0)
            return ServiceResult<object>.BadRequest("invalid_nutrition_type", "Tipo nutrizione non valido.");

        if (request.TotDailyCalories <= 0)
            return ServiceResult<object>.BadRequest("invalid_calories", "Le calorie giornaliere devono essere maggiori di zero.");

        // Controllo pasti
        if (request.Meals == null || request.Meals.Count == 0)
            return ServiceResult<object>.BadRequest("missing_meals", "Aggiungi almeno un pasto al piano nutrizionale.");

        if (request.Meals.Any(m => m.MealId <= 0 || m.DayId <= 0))
            return ServiceResult<object>.BadRequest("invalid_meals", "Uno o più pasti selezionati non sono validi.");

        // Controllo date
        if (request.CreationDate == default)
            request.CreationDate = DateTime.UtcNow.Date;

        // Verifica esistenza utente e tipo nutrizione nel DB
        var userExists = await _context.Users.AnyAsync(u => u.UserId == request.UserId);
        if (!userExists)
            return ServiceResult<object>.BadRequest("user_not_found", "Utente non trovato.");

        var typeExists = await _context.TypeNutritions.AnyAsync(t => t.TypeNutritionId == request.TypeNutritionId);
        if (!typeExists)
            return ServiceResult<object>.BadRequest("nutrition_type_not_found", "Tipo nutrizione non trovato.");

        // Creazione oggetto Nutrition
        var nutrition = new Nutrition
        {
            UserId = request.UserId,
            TypeNutritionId = request.TypeNutritionId,
            TotDailyCalories = request.TotDailyCalories,
            CreationDate = request.CreationDate.Date,
            ExpirationDate = request.ExpirationDate?.Date,
            NutritionMeals = [.. request.Meals.Select(m => new NutritionMeal
            {
                MealId = m.MealId,
                DayId = m.DayId
            })]
        };

        try
        {
            _context.Nutritions.Add(nutrition);
            await _context.SaveChangesAsync();
            return ServiceResult<object>.Ok(new { message = "Piano nutrizionale creato con successo.", nutritionId = nutrition.NutritionId });
        }
        catch (DbUpdateException)
        {
            return ServiceResult<object>.Error("nutrition_save_failed", "Non è stato possibile salvare il piano nutrizionale. Riprova.");
        }
    }

    public async Task<ServiceResult<object>> UpdateAsync(int id, NutritionResPost request)
    {
        var nutrition = await _context.Nutritions
            .Include(n => n.NutritionMeals)
            .FirstOrDefaultAsync(n => n.NutritionId == id && n.ExpirationDate == null);

        if (nutrition == null)
            return ServiceResult<object>.NotFound("nutrition_not_found", "Piano nutrizionale non trovato.");

        nutrition.TypeNutritionId = request.TypeNutritionId;
        nutrition.TotDailyCalories = request.TotDailyCalories;
        nutrition.CreationDate = request.CreationDate.Date;
        nutrition.ExpirationDate = request.ExpirationDate?.Date;

        nutrition.NutritionMeals.Clear();
        foreach (var newMeal in request.Meals)
        {
            nutrition.NutritionMeals.Add(new NutritionMeal
            {
                MealId = newMeal.MealId,
                DayId = newMeal.DayId
            });
        }

        try
        {
            await _context.SaveChangesAsync();
            return ServiceResult<object>.Ok("Piano nutrizionale aggiornato con successo.");
        }
        catch (Exception)
        {
            return ServiceResult<object>.Error("nutrition_update_failed", "Non è stato possibile aggiornare il piano nutrizionale. Riprova.");
        }
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id)
    {
        var nutrition = await _context.Nutritions
            .Include(n => n.NutritionMeals)
            .FirstOrDefaultAsync(n => n.NutritionId == id);

        if (nutrition == null)
            return ServiceResult<object>.NotFound("nutrition_not_found", "Piano nutrizionale non trovato.");

        try
        {
            // Soft-delete: impostiamo la data di eliminazione
            nutrition.ExpirationDate = DateTime.UtcNow.Date;

            await _context.SaveChangesAsync();
            return ServiceResult<object>.Ok("Piano nutrizionale eliminato con successo.");
        }
        catch (Exception)
        {
            return ServiceResult<object>.Error("nutrition_delete_failed", "Non è stato possibile eliminare il piano nutrizionale. Riprova.");
        }
    }

    private static NutritionRes ToRes(Nutrition n) => new()
    {
        NutritionId = n.NutritionId,
        UserId = n.UserId,
        TypeNutritionId = n.TypeNutritionId,
        TotDailyCalories = n.TotDailyCalories,
        CreationDate = n.CreationDate ?? default,
        ExpirationDate = n.ExpirationDate,
        Meals = [.. n.NutritionMeals.Select(nm => new MealRes
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
            CategoryId = nm.Meal.CategoryId,
            DayId = nm.DayId
        })]
    };
}
