using Microsoft.AspNetCore.Mvc;
using ModelsDTO.Payments;
using Payments.ModelsDB;

namespace Payments.Controllers
{
    [ApiController]
    [Route("/api/v1/payment")]
    public class PaymentsAPIController : ControllerBase
    {
        private readonly PaymentsWebController _paymentsController;

        public PaymentsAPIController(PaymentsWebController paymentsController)
        {
            _paymentsController = paymentsController;
        }

        private Payment GetPaymentFromInfo(PaymentInfo paymentInfo)
        {
            var payment = new Payment()
            {
                Id = 0,
                PaymentUid = paymentInfo.PaymentUid,
                Status = paymentInfo.Status,
                Price = paymentInfo.Price
            };
            return payment;
        }

        private PaymentInfo InitPaymentInfo(Payment payment)
        {
            PaymentInfo paymentInfo = new PaymentInfo()
            {
                PaymentUid = payment.PaymentUid,
                Status = payment.Status,
                Price = payment.Price
            };
            return paymentInfo;
        }

        /// <summary>Получить оплату по Uuid</summary>
        [HttpGet("{paymentUid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaymentInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaymentByUid(Guid paymentUid)
        {
            var payment = await _paymentsController.GetPaymentByUid(paymentUid);
            var response = InitPaymentInfo(payment);
            
            return Ok(response);
        }
        
        /// <summary> Создание новой оплаты </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PaymentInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentInfo paymentInfo)
        {
            var paymentToAdd = GetPaymentFromInfo(paymentInfo);
            var addedPayment = await _paymentsController.AddPayment(paymentToAdd);
            
            var response = InitPaymentInfo(addedPayment);
            return Created($"/api/v1/{addedPayment.Id}", response);
        }
        
        [HttpPatch("{rentalUid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaymentInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CancelRental(Guid rentalUid)
        {
            var payment = await _paymentsController.GetPaymentByUid(rentalUid);
            payment.Status = "CANCELED";
            await _paymentsController.CancelPayment(payment);

            var response = InitPaymentInfo(payment);
            return Ok(response);
        }
    }
}