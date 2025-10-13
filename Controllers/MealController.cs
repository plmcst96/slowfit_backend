using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using slowfit.DBModels;
using slowfit.DTORequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace slowfit.Controllers
{
    [Route("slowfit/meal")]
    [ApiController]
    public class MealController(SlowFitContext slowFitContext) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitContext;

        // GET: api/<MealController>
        [HttpGet]
        public ActionResult<IEnumerable<MealRes>> Get()
        {
            var mealList = new List<MealRes>();
            try
            {

                mealList = [.. _slowFitContext.Meals.Select(t => new MealRes
                {
                    MealId = t.MealId,
                    Name = t.Name,
                    Description = t.Description,
                    Recipe = t.Recipe,
                    Calories = t.Calories,
                    PreparingTime = t.PreparingTime,
                    Protein = t.Protein,
                    Fats = t.Fats,
                    Carbohydrate = t.Carbohydrate,
                    ImageMeal = t.ImageMeal,
                    Difficulty = t.Difficulty,
                    CategoryId = t.CategoryId

                })];


                if (mealList.Count == 0) return NoContent();

                return Ok(mealList);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred");
            }
        }

        [HttpGet("{id}/withIngredients")]
        public ActionResult<MealRes> GetMealWithIngredients(int id)
        {
            try
            {
                var meal = _slowFitContext.Meals
                    .Include(m => m.MealIngredients)
                        .ThenInclude(mi => mi.Ingredient)
                    .FirstOrDefault(m => m.MealId == id);

                if (meal == null)
                    return NotFound($"Nessun pasto trovato con ID: {id}");

                var mealRes = new MealRes
                {
                    MealId = meal.MealId,
                    Name = meal.Name,
                    Description = meal.Description,
                    Recipe = meal.Recipe,
                    ImageMeal = meal.ImageMeal,
                    CategoryId = meal.CategoryId,
                    Carbohydrate = meal.Carbohydrate,
                    Fats = meal.Fats,
                    Protein = meal.Protein,
                    PreparingTime = meal.PreparingTime,
                    Calories = meal.Calories,
                    Difficulty = meal.Difficulty,
                    Ingredients = [.. meal.MealIngredients.Select(mi => new IngredientRes
                    {
                        IngredientId = mi.Ingredient.IngredientId,
                        Name = mi.Ingredient.Name,
                        Quantity = mi.Quantity,
                        Unit = mi.Unit
                    })]
                };

                return Ok(mealRes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore del server: {ex.Message}");
            }
        }


        [HttpGet("byIngredient/{ingredientId}")]
        public ActionResult<IEnumerable<MealRes>> GetMealsByIngredient(int ingredientId)
        {
            var meals = _slowFitContext.MealIngredients
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
                .ToList();

            if (meals.Count == 0)
                return NotFound($"No meals found containing ingredient {ingredientId}");

            return Ok(meals);
        }


        // GET api/<MealController>/5
        [HttpGet("{id}")]
        public ActionResult<MealRes> GetSingleMeal(int id)
        {
            try
            {
                var meal = _slowFitContext.Meals.Where(t => t.MealId == id).FirstOrDefault();
                if (meal == null) return NotFound();
                return Ok(meal);
            }
            catch (Exception ex)
            {
                return BadRequest($"No meal found with {id}");
            }
        }

        // GET slowfit/meal/byCategory/{categoryId}
        [HttpGet("byCategory/{categoryId}")]
        public ActionResult<IEnumerable<MealRes>> GetMealsByCategory(int categoryId)
        {
            try
            {
                var meals = _slowFitContext.Meals
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
                    .ToList();

                if (meals.Count == 0)
                    return NotFound($"No meals found for category {categoryId}");

                return Ok(meals);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while fetching meals for category {categoryId}");
            }
        }

        // GET slowfit/meal/byDay/{dayId}
        [HttpGet("byDay/{dayId}")]
        public ActionResult<IEnumerable<MealRes>> GetMealsByDay(int dayId)
        {
            try
            {
                // Recupera tutti i pasti associati al dayId tramite NutritionMeals
                var meals = _slowFitContext.NutritionMeals
                    .Where(nm => nm.DayId == dayId)
                    .Include(nm => nm.Meal) // Include i dettagli del pasto
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
                    .Distinct() // Evita duplicati se più NutritionMeals puntano allo stesso pasto
                    .ToList();

                if (meals.Count == 0)
                    return NotFound($"Nessun pasto trovato per il giorno con ID {dayId}");

                return Ok(meals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore del server: {ex.Message}");
            }
        }


        // Aggiungere una ricetta con i suoi ingredienti
        [HttpPost]
        public IActionResult CreateMealWithIngredients([FromBody] MealResWithIngredients mealWithIngredients)
        {
            // ✅ Controllo se il body è nullo
            if (mealWithIngredients == null)
                return BadRequest("Il corpo della richiesta non può essere nullo.");

            // ✅ Controllo campi obbligatori
            if (string.IsNullOrWhiteSpace(mealWithIngredients.Name))
                return BadRequest("Il nome del pasto è obbligatorio.");
            if (mealWithIngredients.Ingredients == null || mealWithIngredients.Ingredients.Count == 0)
                return BadRequest("Devi fornire almeno un ingrediente.");

            try
            {
                // ✅ Creazione entità Meal
                var meal = new Meal
                {
                    Name = mealWithIngredients.Name,
                    Description = mealWithIngredients.Description,
                    Recipe = mealWithIngredients.Recipe,
                    Calories = mealWithIngredients.Calories,
                    Protein = mealWithIngredients.Protein,
                    Fats = mealWithIngredients.Fats,
                    Carbohydrate = mealWithIngredients.Carbohydrate,
                    PreparingTime = mealWithIngredients.PreparingTime,
                    ImageMeal = mealWithIngredients.ImageMeal,
                    Difficulty = mealWithIngredients.Difficulty,
                    CategoryId = mealWithIngredients.CategoryId
                };

                _slowFitContext.Meals.Add(meal);
                _slowFitContext.SaveChanges(); // 🔥 Salvataggio iniziale per ottenere l'ID

                // ✅ Inserimento ingredienti con controlli
                foreach (var ingredient in mealWithIngredients.Ingredients)
                {
                    if (ingredient.IngredientId <= 0)
                        return BadRequest($"Ingrediente non valido con ID: {ingredient.IngredientId}");

                    if (ingredient.Quantity <= 0)
                        return BadRequest($"La quantità dell'ingrediente ID {ingredient.IngredientId} deve essere maggiore di zero.");

                    var mealIngredient = new MealIngredient
                    {
                        MealId = meal.MealId,
                        IngredientId = ingredient.IngredientId,
                        Quantity = ingredient.Quantity,
                        Unit = ingredient.Unit!
                    };
                    _slowFitContext.MealIngredients.Add(mealIngredient);
                }

                _slowFitContext.SaveChanges();

                return Ok(new
                {
                    message = "Meal creato con successo.",
                    mealId = meal.MealId
                });
            }
            catch (DbUpdateException dbEx)
            {
                // 🔥 Errori relativi al database (es. vincoli FK)
                return StatusCode(500, $"Errore durante il salvataggio nel database: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                // 🔥 Gestione di qualsiasi altra eccezione
                return StatusCode(500, $"Si è verificato un errore imprevisto: {ex.Message}");
            }
        }

        // Modificare una ricetta (aggiornare gli ingredienti)
        [HttpPut("{id}")]
        public IActionResult UpdateMealWithIngredients(int id, [FromBody] MealResWithIngredients updateMeal)
        {
            if (updateMeal == null)
                return BadRequest("Il corpo della richiesta non può essere nullo.");
            if (string.IsNullOrWhiteSpace(updateMeal.Name))
                return BadRequest("Il nome della ricetta è obbligatorio.");
            if (updateMeal.Ingredients == null || updateMeal.Ingredients.Count == 0)
                return BadRequest("Devi fornire almeno un ingrediente.");

            try
            {
                var meal = _slowFitContext.Meals.FirstOrDefault(m => m.MealId == id);
                if (meal == null)
                    return NotFound($"Nessuna ricetta trovata con ID {id}.");

                // ✅ Aggiornamento dei campi base
                meal.Name = updateMeal.Name;
                meal.Description = updateMeal.Description;
                meal.Recipe = updateMeal.Recipe;
                meal.Calories = updateMeal.Calories;
                meal.Protein = updateMeal.Protein;
                meal.Fats = updateMeal.Fats;
                meal.Carbohydrate = updateMeal.Carbohydrate;
                meal.PreparingTime = updateMeal.PreparingTime;
                meal.ImageMeal = updateMeal.ImageMeal;
                meal.Difficulty = updateMeal.Difficulty;
                meal.CategoryId = updateMeal.CategoryId;

                // ✅ Rimuovere gli ingredienti esistenti
                var existingIngredients = _slowFitContext.MealIngredients.Where(mi => mi.MealId == id);
                _slowFitContext.MealIngredients.RemoveRange(existingIngredients);

                // ✅ Aggiungere nuovi ingredienti con controlli
                foreach (var ingredient in updateMeal.Ingredients)
                {
                    if (ingredient.IngredientId <= 0)
                        return BadRequest($"Ingrediente non valido (ID: {ingredient.IngredientId})");
                    if (ingredient.Quantity <= 0)
                        return BadRequest($"La quantità dell'ingrediente con ID {ingredient.IngredientId} deve essere > 0.");

                    var mealIngredient = new MealIngredient
                    {
                        MealId = id,
                        IngredientId = ingredient.IngredientId,
                        Quantity = ingredient.Quantity,
                        Unit = ingredient.Unit ?? "g"
                    };

                    _slowFitContext.MealIngredients.Add(mealIngredient);
                }

                _slowFitContext.SaveChanges();
                return Ok(new { message = "Ricetta aggiornata con successo.", mealId = id });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Errore durante l'aggiornamento nel database: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore imprevisto: {ex.Message}");
            }
        }

        // Eliminare una ricetta e i suoi ingredienti
        [HttpDelete("{id}")]
        public IActionResult DeleteMeal(int id)
        {
            if (id <= 0)
                return BadRequest("ID non valido.");

            try
            {
                var meal = _slowFitContext.Meals.FirstOrDefault(m => m.MealId == id);
                if (meal == null)
                    return NotFound($"Nessuna ricetta trovata con ID {id}.");

                // ✅ Eliminare ingredienti associati
                var ingredients = _slowFitContext.MealIngredients.Where(mi => mi.MealId == id);
                _slowFitContext.MealIngredients.RemoveRange(ingredients);

                // ✅ Eliminare la ricetta
                _slowFitContext.Meals.Remove(meal);
                _slowFitContext.SaveChanges();

                return Ok(new { message = "Ricetta e relativi ingredienti eliminati con successo.", mealId = id });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Errore durante l'eliminazione dal database: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore imprevisto: {ex.Message}");
            }
        }

    }
}
