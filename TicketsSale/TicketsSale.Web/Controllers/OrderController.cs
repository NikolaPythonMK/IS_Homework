using GemBox.Document;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TicketsSale.Domain.Identity;
using TicketsSale.Service.Interface;

namespace TicketsSale.Web.Controllers
{
    public class OrderController : Controller
    {

        private readonly UserManager<TicketsSaleApplicationUser> _userManager;
        private readonly IOrderService _orderService;


        public OrderController(IOrderService orderService, UserManager<TicketsSaleApplicationUser> userManager)
        {
            _userManager = userManager;
            _orderService = orderService;
        }

        [Authorize]
        public IActionResult Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier.ToString()).Value;
            return View(_orderService.GetAll(userId));
        }


        public IActionResult CreateInvoice(Guid id)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Invoice.docx");
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
            var order = _orderService.Get(new Domain.BaseEntity { Id = id });
            var document = DocumentModel.Load(templatePath);

            document.Content.Replace("{{OrderNumber}}", order.Id.ToString());
            document.Content.Replace("{{CostumerEmail}}", order.User.Email);
            document.Content.Replace("{{CostumerInfo}}", (order.User.UserName + " " + order.User.PhoneNumber));
            
            StringBuilder sb = new StringBuilder();           
            var total_price = order.TicketInOrder.Sum(item => item.Quantity * item.Ticket.Price);

            foreach (var item in order.TicketInOrder)
            {
                sb.AppendLine(item.Ticket.MovieTitle + " with quantity " + item.Quantity + ", and price " + item.Ticket.Price + "$");
            }

            document.Content.Replace("{{AllTickets}}", sb.ToString());
            document.Content.Replace("{{TotalPrice}}", "$" + total_price.ToString());
            var stream = new MemoryStream();
            document.Save(stream, new PdfSaveOptions());
            return File(stream.ToArray(), new PdfSaveOptions().ContentType, "ExportInvoice.pdf");
        }
    }
}
