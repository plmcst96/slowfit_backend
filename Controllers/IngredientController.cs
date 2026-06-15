using Microsoft.AspNetCore.Mvc;
using slowfit.DBModels;
using slowfit.DTORequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace slowfit.Controllers
{
    [Route("slowfit/ingredient")]
    [ApiController]
    public class IngredientController(SlowFitContext slowFitCtx) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitCtx;
        // GET: api/<IngredientController>
        [HttpGet]
        public ActionResult<IEnumerable<IngredientRes>> GetAll()
        {
            var ingredientList = new List<IngredientRes>();
            try
            {
                ingredientList = _slowFitContext.Ingredients.Select(t => new IngredientRes
                {
                    IngredientId = t.IngredientId,
                    Name = t.Name,
                    Calories = t.Calories,
                    Protein = t.Protein,
                    Fats = t.Fats,
                    Carbohydrate = t.Carbohydrate,

                }).ToList();

                if (ingredientList.Count == 0) return NoContent();
                return Ok(ingredientList);
            }
            catch (Exception)
            {
                return BadRequest($"An error occurred");
            }
        }

        [HttpGet("search")]
        public ActionResult<IEnumerable<IngredientRes>> GetBySearchQuery([FromQuery] string name)
        {
            var ingredientList = new List<IngredientRes>();
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return BadRequest("The 'name' parameter cannot be empty");

                ingredientList = _slowFitContext.Ingredients
                .Where(i => i.Name.ToLower().Contains(name.ToLower()))
                .Select(t => new IngredientRes
                {
                    IngredientId = t.IngredientId,
                    Name = t.Name,
                    Calories = t.Calories,
                    Protein = t.Protein,
                    Fats = t.Fats,
                    Carbohydrate = t.Carbohydrate,
                })
                .ToList();


                if (ingredientList.Count == 0)
                    return NotFound($"No ingredients found containing '{name}'");

                return Ok(ingredientList);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }



        // GET api/<IngredientController>/5
        [HttpGet("{id}")]
        public ActionResult<IngredientRes> GetSingleIngredient(int id)
        {
            try
            {
                var ingredient = _slowFitContext.Ingredients.Where(t => t.IngredientId == id).FirstOrDefault();
                if (ingredient == null) return NotFound();
                return Ok(ingredient);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        // POST api/<IngredientController>
        [HttpPost]
        public IActionResult CreateIngredient([FromBody] IngredientRes ingredient)
        {
            if (ingredient == null)
            {
                return BadRequest("Ingredient cannot be null");
            }

            if (string.IsNullOrEmpty(ingredient.Name))
              
            {
                return BadRequest("Ingredient name cannot be null or empty");
            }

            try
            {
                var newIngredient = new Ingredient
                {
                    Name = ingredient.Name,
                    Calories = ingredient.Calories,
                    Carbohydrate = ingredient.Carbohydrate,
                    Protein = ingredient.Protein,
                    Fats = ingredient.Fats
                };
                _slowFitContext.Ingredients.Add(newIngredient);
                _slowFitContext.SaveChanges();
                return Ok("Ingredient created succesfully");
            }
            catch (Exception)
            {
                return BadRequest($"Failed to create ingredient");
            }
        }

        // PUT api/<IngredientController>/5
        [HttpPut("{id}")]
        public IActionResult UpdateIngredient(int id, [FromBody] IngredientRes updateIngredient)
        {
            var ingredient = _slowFitContext.Ingredients.Where(t => t.IngredientId == id).FirstOrDefault();
            if (ingredient == null) return NotFound();
            ingredient.Name = updateIngredient.Name;
            ingredient.Calories = updateIngredient.Calories;
            ingredient.Protein = updateIngredient.Protein;
            ingredient.Fats = updateIngredient.Fats;
            ingredient.Carbohydrate = updateIngredient.Carbohydrate;
            try
            {
                _slowFitContext.Ingredients.Update(ingredient);
                _slowFitContext.SaveChanges();
                return Ok("Ingredient updated succesfully");
            }
            catch (Exception)
            {
                return BadRequest($"Failed to update ingredient: {id}");
            }
        }

        // DELETE api/<IngredientController>/5
        [HttpDelete("{id}")]
        public IActionResult DeleteIngredient(int id)
        {
            var ingredient = _slowFitContext.Ingredients.Where(t => t.IngredientId == id).FirstOrDefault();
            if (ingredient == null) return NotFound();
            try
            {
                _slowFitContext.Ingredients.Remove(ingredient);
                _slowFitContext.SaveChanges();
                return Ok("Ingredient deleted successfully");
            }
            catch (Exception)
            {
                return BadRequest($"Failed to delete ingredient: {id}");
            }
        }
    }
}
