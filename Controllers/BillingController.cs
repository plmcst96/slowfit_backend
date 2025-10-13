using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using slowfit.DBModels;
using slowfit.DTORequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace slowfit.Controllers
{
    [Route("slowFit/billing")]
    [ApiController]
    public class BillingController(SlowFitContext slowFitContext) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitContext;

        // GET: api/<BillingController>
        [HttpGet]
        public ActionResult<IEnumerable<BillingRes>> Get()
        {
            var billingList = new List<BillingRes>();
            try
            {

                billingList = _slowFitContext.Billings.Select(t => new BillingRes
                {
                    BillingId = t.BillingId,
                    PaymentTypeId = t.PaymentTypeId,
                    OrderId = t.OrderId,
                    UserId = t.UserId,
                    Date = DateTime.ParseExact(t.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Amount = t.Amount,

                }).ToList();


                if (billingList.Count == 0) return NoContent();

                return Ok(billingList);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred");
            }
        }

        // GET api/<BillingController>/5
        [HttpGet("{id}")]
        public ActionResult<BillingRes> Get(int id)
        {
            try
            {
                var billing = _slowFitContext.Billings.Where(t => t.BillingId == id).FirstOrDefault();
                if (billing == null) return NotFound();
                return Ok(billing);
            }
            catch (Exception ex)
            {
                return BadRequest($"No billing found with {id}");
            }
        }

        [HttpGet("byUser/{userId}")]
        public ActionResult<BillingRes> GetBillingByUserId(int userId)
        {
            try
            {
                var billing = _slowFitContext.Billings.Where(t => t.UserId == userId).ToList();
                if (billing == null) return NotFound();
                return Ok(billing);
            }
            catch (Exception ex)
            {
                return BadRequest($"No billing found with {userId}");
            }
        }

        // POST api/<BillingController>
        [HttpPost]
        public IActionResult CreatBilling([FromBody] BillingRes billing)
        {
            if (billing == null)
            {
                return BadRequest("Invalid request body.");
            }

            if (
                billing.PaymentTypeId <= 0 ||
                billing.OrderId <= 0 ||
                billing.UserId <= 0 ||
                billing.Date == default ||
                billing.Amount <= 0)
            {
                return BadRequest();
            }

            try
            {
                var bil = new Billing()
                {
                    PaymentTypeId = billing.PaymentTypeId,
                    OrderId = billing.OrderId,
                    UserId = billing.UserId,
                    Date = billing.Date.ToString("yyyy-MM-dd"),
                    Amount = billing.Amount
                };
                _slowFitContext.Billings.Add(bil);
                _slowFitContext.SaveChanges();
                return Ok("Billing created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create billing");
            }
        }
    }
}
