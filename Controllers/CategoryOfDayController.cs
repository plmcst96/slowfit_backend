using Microsoft.AspNetCore.Mvc;
using slowfit.DBModels;
using slowfit.DTORequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace slowfit.Controllers
{
    [Route("slowfit/category")]
    [ApiController]
    public class CategoryOfDayController(SlowFitContext slowFitCtx) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitCtx;


        [HttpGet]
        public ActionResult<IEnumerable<CategoryOfDayRes>> GetAll()
        {
            var categoryList = new List<CategoryOfDayRes>();
            try
            {

                categoryList = _slowFitContext.CategoryOfDays.Select(c => new CategoryOfDayRes
                {
                    CategoryId = c.CategoryId,
                    MomentOfDay = c.MomentOfDay,
                }).ToList();

                if (categoryList.Count == 0) return NoContent();

                return Ok(categoryList);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred");
            }
        }

        // GET slowfit/category/{id}
        [HttpGet("{id}")]
        public ActionResult<CategoryOfDayRes> GetById(int id)
        {
            try
            {
                var category = _slowFitContext.CategoryOfDays
                    .Where(c => c.CategoryId == id)
                    .Select(c => new CategoryOfDayRes
                    {
                        CategoryId = c.CategoryId,
                        MomentOfDay = c.MomentOfDay
                    })
                    .FirstOrDefault();

                if (category == null)
                {
                    return NotFound(new { message = "Category not found" });
                }

                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while fetching the category: {ex.Message}");
            }
        }


        [HttpPost]
        public IActionResult Post([FromBody] CategoryOfDayRes category)
        {
            if (category == null)
            {
                return BadRequest("Data type incorrect");
            }

            if (string.IsNullOrEmpty(category.MomentOfDay))
            {
                return BadRequest();
            }
            try
            {
                var cat = new CategoryOfDay()
                {
                    MomentOfDay = category.MomentOfDay,
                };
                _slowFitContext.CategoryOfDays.Add(cat);
                _slowFitContext.SaveChanges();
                return Ok("Category created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create category");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] CategoryOfDayRes category)
        {
            if (category == null || category.CategoryId != id)
            {
                return BadRequest("Invalid data or ID does not match");
            }

            var existingCtegory = _slowFitContext.CategoryOfDays.Where(u => u.CategoryId == id).FirstOrDefault();
            if (existingCtegory == null)
            {
                return NotFound("Plan not found");
            }

            // Aggiorna i dati
            existingCtegory.MomentOfDay = category.MomentOfDay;


            try
            {
                _slowFitContext.CategoryOfDays.Update(existingCtegory);


                _slowFitContext.SaveChanges();

                return Ok("Category updated succesfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update category: {category.MomentOfDay}");
            }
        }

        // DELETE api/<TyepeTrainingController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var category = _slowFitContext.CategoryOfDays.Where(u => u.CategoryId == id).FirstOrDefault();

            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }

            try
            {
                _slowFitContext.CategoryOfDays.Remove(category);
                _slowFitContext.SaveChanges();
                return Ok("Category delete succesfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error to delete category {category.MomentOfDay} ");
            }
        }
    }
}
