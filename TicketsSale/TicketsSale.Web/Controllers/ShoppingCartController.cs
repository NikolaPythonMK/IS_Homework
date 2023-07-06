using Microsoft.AspNetCore.Mvc;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TicketsSale.Service.Interface;

namespace TicketsSale.Web.Controllers
{
    public class ShoppingCartController : Controller
    {

        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        public IActionResult Index()
        {
            var User_ID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return View(_shoppingCartService.Find(User_ID));
        }


        public IActionResult DeleteFromShoppingCart(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _shoppingCartService.DeleteTicket(userId, id);
            return RedirectToAction("Index", "ShoppingCart");
        }

        public IActionResult PayOrder(string stripeEmail, string stripeToken)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = _shoppingCartService.Find(userId);

            var customerService = new CustomerService();
            var customer = customerService.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                Source = stripeToken
            }); 

            var chargeService = new ChargeService();
            var charge = chargeService.Create(new ChargeCreateOptions
            {
                Amount = (int)(order.TotalPrice * 100),
                Description = "Cinema Application Payment",
                Currency = "usd",
                Customer = customer.Id
            });

            if (charge.Status == "succeeded")
            {
                MakeOrder();
            }

            return RedirectToAction("Index", "ShoppingCart");
        }


        private Boolean MakeOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = _shoppingCartService.Order(userId);
            return result;
        }
    }
}
