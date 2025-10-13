using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using slowfit.DBModels;
using slowfit.DTORequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace slowfit.Controllers
{
    [Route("slowFit/product")]
    [ApiController]
    public class ProductController(SlowFitContext slowFitCtx) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitCtx ;

        // GET: api/<ProductRes>
        [HttpGet]
        public ActionResult<IEnumerable<ProductRes>> GetAllProducts()
        {
            var productList = new List<ProductRes>();
            try
            {

                 productList = _slowFitContext.Products.Select(p => new ProductRes
                {
                    Description = p.Description,
                    Name = p.Name,
                    ExpirationDate = DateTime.ParseExact(p.ExpirationDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    ProductId = p.ProductId,
                    Price = p.Price,
                }).ToList();

                if (productList.Count == 0) return NoContent();

                return Ok(productList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }

        }

        // GET api/<ProductRes>/5
        [HttpGet("{id}")]
        public ActionResult<ProductRes> GetSingleProduct(int id)
        {
            try
            {
                var product = _slowFitContext.Products.Where(t => t.ProductId == id).FirstOrDefault();
                if (product == null) return NotFound();

                return Ok(product);
            } catch (Exception ex)
            {
                return BadRequest("No product found");
            }
        }


        [HttpPost]
        public ActionResult CreateProduct([FromBody] ProductRes product)
        {
            if (product == null)
            {
                return BadRequest("Invalid request body.");
            }

            if (string.IsNullOrEmpty(product.Name) || product.Price == 0 || string.IsNullOrEmpty(product.Description))
            {
                return BadRequest();
            }

            if (product.ExpirationDate == default)
            {
                return BadRequest();
            }

            try
            {
                var prod = new Product()
                {
                    Description = product.Description,
                    Name = product.Name,
                    ExpirationDate = product.ExpirationDate.ToString("yyyy-MM-dd"),
                    Price = product.Price
                };
                _slowFitContext.Products.Add(prod);
                _slowFitContext.SaveChanges();
                return Ok("Product created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create product");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] ProductRes updatedProduct)
        {
            var product = _slowFitContext.Products.Where(p => p.ProductId == id).FirstOrDefault();
            if (product == null) return NotFound();



            product.ExpirationDate = updatedProduct.ExpirationDate.ToString("yyyy-MM-dd");
            product.Name = updatedProduct.Name;
            product.Price = updatedProduct.Price;
            product.Description = updatedProduct.Description;
           

            try
            {
                _slowFitContext.Products.Update(product);


                _slowFitContext.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update product: {updatedProduct.Name}");
            }

            }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _slowFitContext.Products.Where(t => t.ProductId == id).FirstOrDefault();
            if (product == null) return NotFound();
            try
            {
                _slowFitContext.Products.Remove(product);
                _slowFitContext.SaveChanges();
                return NoContent();
            }
            catch (Exception ex) {
                return BadRequest($"Error to delete product {product.Name} ");
            }
            
        }
    }
}
