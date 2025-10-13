using Microsoft.AspNetCore.Mvc;
using slowfit.DBModels;
using slowfit.DTORequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace slowfit.Controllers
{
    [Route("slowFit/order")]
    [ApiController]
    public class OrderController(SlowFitContext slowFitCtx) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitCtx;

        // GET: api/<OrderController>
        [HttpGet]
        public ActionResult<IEnumerable<OrderRes>> GetAll()
        {
            var orderList = new List<OrderRes>();
            try
            {

                orderList = _slowFitContext.Orders.Select(t => new OrderRes
                {
                   OrderId = t.OrderId,
                   UserId = t.UserId,
                   PaymentTypeId = t.PaymentTypeId,
                   ProductId = t.ProductId,
                   Amount = t.Amount,
                }).ToList();


                if (orderList.Count == 0) return NoContent();

                return Ok(orderList);
            }
            catch (Exception ex)
            {
                return BadRequest( $"An error occurred");
            }
        }

        // GET api/<OrderController>/5
        [HttpGet("{id}")]
        public ActionResult<OrderRes> GetSingleOrder(int id)
        {
            try
            {
                var order = _slowFitContext.Orders.Where(t => t.OrderId == id).FirstOrDefault();
                if (order == null) return NotFound();
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest($"No order found with {id}");
            }
        }

        [HttpGet("byUser/{userId}")]
        public ActionResult<IEnumerable<OrderRes>> GetOrderByUserId(int userId)
        {
            try
            {
                var userOrder = _slowFitContext.Orders.Where(t => t.UserId == userId).ToList();
                if (userOrder.Count == 0) return NotFound($"No trainings found for user {userId}.");
                return Ok(userOrder);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        // POST api/<OrderController>
        [HttpPost]
        public IActionResult CreateOrder([FromBody] OrderRes order)
        {
            if (order == null)
            {
                return BadRequest("Invalid request body.");
            }

            if (order.OrderId <= 0 || order.PaymentTypeId <= 0 || order.UserId <= 0 || order.ProductId <= 0 || order.Amount <= 0)
            {
                return BadRequest();
            }

            try
            {
                var or = new Order()
                {
                    ProductId = order.ProductId,
                    Amount = order.Amount,
                    PaymentTypeId = order.PaymentTypeId,
                    UserId = order.UserId,
                };
                _slowFitContext.Orders.Add(or);
                _slowFitContext.SaveChanges();
                return Ok("Order created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create order");
            }
        }

        // PUT api/<OrderController>/5
        [HttpPut("{id}")]
        public IActionResult UpdateOrder(int id, [FromBody] OrderRes orderBody)
        {
            var order = _slowFitContext.Orders.Where(t => t.OrderId == id).FirstOrDefault();
            if (order == null) return NotFound();

            order.ProductId = orderBody.ProductId;
            order.Amount = orderBody.Amount;
            order.PaymentTypeId = orderBody.PaymentTypeId;
            order.UserId = orderBody.UserId;
          
            try
            {
                _slowFitContext.Orders.Update(order);


                _slowFitContext.SaveChanges();

                return Ok("Order updated succesfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update order: {orderBody.OrderId}");
            }
        }

        // DELETE api/<OrderController>/5
        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id)
        {
            var order = _slowFitContext.Orders.Where(t => t.OrderId == id).FirstOrDefault();
            if (order == null) return NotFound();

            try
            {
                _slowFitContext.Orders.Remove(order);
                _slowFitContext.SaveChanges();

                return Ok($"The order has been successfully cancelled");
            }
            catch (Exception ex)
            {
                return BadRequest($"No order found whit {id}");
            }
        }
    }
}
