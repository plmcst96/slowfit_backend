using System;
using Microsoft.AspNetCore.Mvc;
using slowfit.DBModels;
using slowfit.DTORequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace slowfit.Controllers
{
    [Route("slowFit/paymentType")]
    [ApiController]
    public class PaymentTypeController(SlowFitContext slowFitCtx) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitCtx;

        // GET: api/<PaymentTypeController>
        [HttpGet]
        public ActionResult<IEnumerable<PaymentTypeRes>> GetAll()
        {
            var paymentList = new List<PaymentTypeRes>();
            try
            {

                paymentList = _slowFitContext.PaymentTypes.Select(p => new PaymentTypeRes
                {
                    PaymentTypeId = p.PaymentTypeId,
                    PaymentTypeName = p.PaymentTypeName,
                }).ToList();

                if (paymentList.Count == 0) return NoContent();

                return Ok(paymentList);
            }
            catch (Exception)
            {
                return BadRequest($"An error occurred");
            }
        }

        // POST api/<PaymentTypeController>
        [HttpPost]
        public IActionResult Post([FromBody] PaymentTypeRes paymentType)
        {
            if (paymentType == null)
            {
                return BadRequest("Data payment incorrect");
            }

            if (string.IsNullOrEmpty(paymentType.PaymentTypeName) || paymentType.PaymentTypeId == 0)
            {
                return BadRequest();
            }
            try
            {
                var pay = new PaymentType()
                {
                    PaymentTypeName = paymentType.PaymentTypeName,
                };
                _slowFitContext.PaymentTypes.Add(pay);
                _slowFitContext.SaveChanges();
                return Ok("Paymnet created successfully.");
            }
            catch (Exception)
            {
                return BadRequest($"Failed to create payment");
            }
        }

        // PUT api/<PaymentTypeController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] PaymentTypeRes paymentType)
        {
            if (paymentType == null || paymentType.PaymentTypeId != id)
            {
                return BadRequest("Invalid data or ID does not match");
            }

            var existingType = _slowFitContext.PaymentTypes.Where(u => u.PaymentTypeId == id).FirstOrDefault();
            if (existingType == null)
            {
                return NotFound("Payment not found");
            }

            // Aggiorna i dati
            existingType.PaymentTypeName = paymentType.PaymentTypeName;


            try
            {
                _slowFitContext.PaymentTypes.Update(existingType);


                _slowFitContext.SaveChanges();

                return Ok("Payment updated succesfully");
            }
            catch (Exception)
            {
                return BadRequest($"Failed to update payment: {paymentType.PaymentTypeName}");
            }
        }

        // DELETE api/<PaymentTypeController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var pay = _slowFitContext.PaymentTypes.Where(u => u.PaymentTypeId == id).FirstOrDefault();

            if (pay == null)
            {
                return NotFound(new { message = "Payment not found" });
            }

            try
            {
                _slowFitContext.PaymentTypes.Remove(pay);
                _slowFitContext.SaveChanges();
                return Ok("Payment delete succesfully");
            }
            catch (Exception)
            {
                return BadRequest($"Error to delete payment {pay.PaymentTypeName} ");
            }
        }
    }
}
