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
            catch (Exception)
            {
                return this.ApiServerError("meal_fetch_failed", "Non è stato possibile caricare le ricette. Riprova.");
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
                    return Ok(new { });

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
            catch (Exception)
            {
                return this.ApiServerError("meal_fetch_failed", "Non è stato possibile caricare la ricetta. Riprova.");
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

            return Ok(meals);
        }


        // GET api/<MealController>/5
        [HttpGet("{id}")]
        public ActionResult<MealRes> GetSingleMeal(int id)
        {
            try
            {
                var meal = _slowFitContext.Meals.Where(t => t.MealId == id).FirstOrDefault();
                if (meal == null) return Ok(new { });
                return Ok(meal);
            }
            catch (Exception)
            {
                return this.ApiNotFound("meal_not_found", "Ricetta non trovata.");
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

                return Ok(meals);
            }
            catch (Exception)
            {
                return this.ApiServerError("meal_fetch_failed", "Non è stato possibile caricare le ricette per questa categoria.");
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

                return Ok(meals);
            }
            catch (Exception)
            {
                return this.ApiServerError("meal_fetch_failed", "Non è stato possibile caricare le ricette per questo giorno.");
            }
        }


        // Aggiungere una ricetta con i suoi ingredienti
        [HttpPost]
        public IActionResult CreateMealWithIngredients([FromBody] MealResWithIngredients mealWithIngredients)
        {
            // ✅ Controllo se il body è nullo
            if (mealWithIngredients == null)
                return this.ApiBadRequest("invalid_meal", "I dati della ricetta sono obbligatori.");

            // ✅ Controllo campi obbligatori
            if (string.IsNullOrWhiteSpace(mealWithIngredients.Name))
                return this.ApiBadRequest("missing_meal_name", "Il nome della ricetta è obbligatorio.");
            if (mealWithIngredients.Ingredients == null || mealWithIngredients.Ingredients.Count == 0)
                return this.ApiBadRequest("missing_ingredients", "Aggiungi almeno un ingrediente.");

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
                        return this.ApiBadRequest("invalid_ingredient", "Uno o più ingredienti selezionati non sono validi.");

                    if (ingredient.Quantity <= 0)
                        return this.ApiBadRequest("invalid_ingredient_quantity", "La quantità degli ingredienti deve essere maggiore di zero.");

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
            catch (DbUpdateException)
            {
                // 🔥 Errori relativi al database (es. vincoli FK)
                return this.ApiServerError("meal_save_failed", "Non è stato possibile salvare la ricetta. Riprova.");
            }
            catch (Exception)
            {
                // 🔥 Gestione di qualsiasi altra eccezione
                return this.ApiServerError();
            }
        }

        // Modificare una ricetta (aggiornare gli ingredienti)
        [HttpPut("{id}")]
        public IActionResult UpdateMealWithIngredients(int id, [FromBody] MealResWithIngredients updateMeal)
        {
            if (updateMeal == null)
                return this.ApiBadRequest("invalid_meal", "I dati della ricetta sono obbligatori.");
            if (string.IsNullOrWhiteSpace(updateMeal.Name))
                return this.ApiBadRequest("missing_meal_name", "Il nome della ricetta è obbligatorio.");
            if (updateMeal.Ingredients == null || updateMeal.Ingredients.Count == 0)
                return this.ApiBadRequest("missing_ingredients", "Aggiungi almeno un ingrediente.");

            try
            {
                var meal = _slowFitContext.Meals.FirstOrDefault(m => m.MealId == id);
                if (meal == null)
                    return this.ApiNotFound("meal_not_found", "Ricetta non trovata.");

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
                        return this.ApiBadRequest("invalid_ingredient", "Uno o più ingredienti selezionati non sono validi.");
                    if (ingredient.Quantity <= 0)
                        return this.ApiBadRequest("invalid_ingredient_quantity", "La quantità degli ingredienti deve essere maggiore di zero.");

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
            catch (DbUpdateException)
            {
                return this.ApiServerError("meal_update_failed", "Non è stato possibile aggiornare la ricetta. Riprova.");
            }
            catch (Exception)
            {
                return this.ApiServerError();
            }
        }

        // Eliminare una ricetta e i suoi ingredienti
        [HttpDelete("{id}")]
        public IActionResult DeleteMeal(int id)
        {
            if (id <= 0)
                return this.ApiBadRequest("invalid_meal", "Ricetta non valida.");

            try
            {
                var meal = _slowFitContext.Meals.FirstOrDefault(m => m.MealId == id);
                if (meal == null)
                    return this.ApiNotFound("meal_not_found", "Ricetta non trovata.");

                // ✅ Eliminare ingredienti associati
                var ingredients = _slowFitContext.MealIngredients.Where(mi => mi.MealId == id);
                _slowFitContext.MealIngredients.RemoveRange(ingredients);

                // ✅ Eliminare la ricetta
                _slowFitContext.Meals.Remove(meal);
                _slowFitContext.SaveChanges();

                return Ok(new { message = "Ricetta e relativi ingredienti eliminati con successo.", mealId = id });
            }
            catch (DbUpdateException)
            {
                return this.ApiServerError("meal_delete_failed", "Non è stato possibile eliminare la ricetta. Riprova.");
            }
            catch (Exception)
            {
                return this.ApiServerError();
            }
        }

    }
}
