using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using slowfit.DBModels;
using slowfit.DTORequest;

namespace slowfit.Controllers
{
    [Route("slowFit/nutrition")]
    [ApiController]
    public class NutritionController(SlowFitContext slowFitContext) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitContext;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NutritionRes>>> GetAll()
        {
            var nutritionList = await _slowFitContext.Nutritions
                .Where(n => n.ExpirationDate == null)
                .Include(n => n.NutritionMeals).ThenInclude(nm => nm.Meal)
                .Select(n => new NutritionRes
                {
                    NutritionId = n.NutritionId,
                    UserId = n.UserId,
                    TypeNutritionId = n.TypeNutritionId,
                    TotDailyCalories = n.TotDailyCalories,
                    CreationDate = n.CreationDate ?? default,
                    ExpirationDate = n.ExpirationDate,
                    Meals = n.NutritionMeals.Select(nm => new MealRes
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
                    }).ToList()
                }).ToListAsync();

            return Ok(nutritionList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NutritionRes>> GetById(int id)
        {
            var nutrition = await _slowFitContext.Nutritions
                .Include(n => n.NutritionMeals).ThenInclude(nm => nm.Meal)
                .FirstOrDefaultAsync(n => n.NutritionId == id && n.ExpirationDate == null);

            if (nutrition == null)
                return Ok(new { });

            var result = new NutritionRes
            {
                NutritionId = nutrition.NutritionId,
                UserId = nutrition.UserId,
                TypeNutritionId = nutrition.TypeNutritionId,
                TotDailyCalories = nutrition.TotDailyCalories,
                CreationDate = nutrition.CreationDate ?? default,
                ExpirationDate = nutrition.ExpirationDate,
                Meals = [.. nutrition.NutritionMeals.Select(nm => new MealRes
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

            return Ok(result);
        }


        [HttpGet("byUser/{userId}")]
        public async Task<ActionResult<IEnumerable<NutritionRes>>> GetByUser(int userId)
        {
            var nutritions = await _slowFitContext.Nutritions
                .Include(n => n.NutritionMeals).ThenInclude(nm => nm.Meal)
                .Where(n => n.UserId == userId && n.ExpirationDate == null)
                .ToListAsync();

            var result = nutritions.Select(n => new NutritionRes
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
            }).ToList();

            return Ok(result);
        }


        // POST crea nuovo piano nutrizionale
        [HttpPost]
        public async Task<IActionResult> CreateNutrition([FromBody] NutritionResPost request)
        {
            // Controllo base: request null
            if (request == null)
                return this.ApiBadRequest("invalid_nutrition", "I dati del piano nutrizionale sono obbligatori.");

            // Controllo proprietà principali
            if (request.UserId <= 0)
                return this.ApiBadRequest("invalid_user", "Utente non valido.");

            if (request.TypeNutritionId <= 0)
                return this.ApiBadRequest("invalid_nutrition_type", "Tipo nutrizione non valido.");

            if (request.TotDailyCalories <= 0)
                return this.ApiBadRequest("invalid_calories", "Le calorie giornaliere devono essere maggiori di zero.");

            // Controllo pasti
            if (request.Meals == null || request.Meals.Count == 0)
                return this.ApiBadRequest("missing_meals", "Aggiungi almeno un pasto al piano nutrizionale.");

            if (request.Meals.Any(m => m.MealId <= 0 || m.DayId <= 0))
                return this.ApiBadRequest("invalid_meals", "Uno o più pasti selezionati non sono validi.");

            // Controllo date
            if (request.CreationDate == default)
                request.CreationDate = DateTime.UtcNow.Date;

            // Optional: verifica esistenza utente e tipo nutrizione nel DB
            var userExists = await _slowFitContext.Users.AnyAsync(u => u.UserId == request.UserId);
            if (!userExists)
                return this.ApiBadRequest("user_not_found", "Utente non trovato.");

            var typeExists = await _slowFitContext.TypeNutritions.AnyAsync(t => t.TypeNutritionId == request.TypeNutritionId);
            if (!typeExists)
                return this.ApiBadRequest("nutrition_type_not_found", "Tipo nutrizione non trovato.");

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
                _slowFitContext.Nutritions.Add(nutrition);
                await _slowFitContext.SaveChangesAsync();
                return Ok(new { message = "Piano nutrizionale creato con successo.", nutritionId = nutrition.NutritionId });
            }
            catch (DbUpdateException)
            {
                // Eccezione specifica EF Core
                return this.ApiServerError("nutrition_save_failed", "Non è stato possibile salvare il piano nutrizionale. Riprova.");
            }
            catch (Exception)
            {
                return this.ApiServerError();
            }
        }


        // PUT aggiorna piano
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNutrition(int id, [FromBody] NutritionResPost request)
        {
            var nutrition = await _slowFitContext.Nutritions
                .Include(n => n.NutritionMeals)
                .FirstOrDefaultAsync(n => n.NutritionId == id && n.ExpirationDate == null);

            if (nutrition == null) return this.ApiNotFound("nutrition_not_found", "Piano nutrizionale non trovato.");

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
                await _slowFitContext.SaveChangesAsync();
                return Ok("Piano nutrizionale aggiornato con successo.");
            }
            catch (Exception)
            {
                return this.ApiServerError("nutrition_update_failed", "Non è stato possibile aggiornare il piano nutrizionale. Riprova.");
            }
        }


        // DELETE soft-delete
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteNutrition(int id)
{
    var nutrition = await _slowFitContext.Nutritions
        .Include(n => n.NutritionMeals)
        .FirstOrDefaultAsync(n => n.NutritionId == id);

    if (nutrition == null) return this.ApiNotFound("nutrition_not_found", "Piano nutrizionale non trovato.");

    try
    {

        // Impostiamo la data di eliminazione
        nutrition.ExpirationDate = DateTime.UtcNow.Date;

        await _slowFitContext.SaveChangesAsync();
        return Ok("Piano nutrizionale eliminato con successo.");
    }
    catch (Exception)
    {
        return this.ApiServerError("nutrition_delete_failed", "Non è stato possibile eliminare il piano nutrizionale. Riprova.");
    }
}

    }
}
