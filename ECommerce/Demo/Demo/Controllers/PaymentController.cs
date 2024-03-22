using Demo.HandleResponses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.OrderService.Dto;
using Services.Services.BasketService.Dto;
using Services.Services.PaymentService;
using Stripe;

namespace Demo.Controllers
{
    
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;
        private const string WhSecret = "whsec_381c31ab4e5994d6dd8b183de2fc092f9bf21b13311ae9b1e1b3f34b85b1dde2";

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpPost("{basketId}")]

       public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntentForExistingOrder(CustomerBasketDto basket, string basketId)
        {
            var customerBasket = await  _paymentService.CreateOrUpdatePaymentIntentForExistingOrder(basket);

            if (customerBasket is null)
                return BadRequest(new ApiResponse(400, "Problem with Your Basket"));

            return Ok(customerBasket); 
        }

        [HttpPost("{basketId}")]

        public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntentForNewOrder(string basketId)
        {
            var customerBasket = await _paymentService.CreateOrUpdatePaymentIntentForNewOrder(basketId);

            if (customerBasket is null)
                return BadRequest(new ApiResponse(400, "Problem with Your Basket"));

            return Ok(customerBasket);
        }
        [HttpPost]

        public async Task<ActionResult> WebHook(string basketId)
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], WhSecret);
                PaymentIntent paymentIntent;
                OrderResultDto order;
                

                
                
                
                // Handle the event
                if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
                {
                    paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("Payment Failed: ", paymentIntent.Id);
                    order = await _paymentService.UpdateOrderPaymentFailed(paymentIntent.Id);
                    _logger.LogInformation("Order Updated To Payment Failed: ", order.Id);
                }
                else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("Payment Succeeded: ", paymentIntent.Id);
                    order = await _paymentService.UpdateOrderPaymentSucceeded(paymentIntent.Id,basketId);
                    _logger.LogInformation("Order Updated To Payment Succeeded: ", order.Id);
                }
                // ... handle other event types
                else
                {
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }

    }
}
