using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TicketsSale.Domain.DomainModels;
using TicketsSale.Domain.DTO;
using TicketsSale.Service.Interface;

namespace TicketsSale.Web.Controllers
{
    public class TicketsController : Controller
    {

        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public IActionResult Index(string DateFilter)
        {
            if (DateFilter != null)
            {
                return View(this._ticketService.GetAllTicketsFromDate(DateTime.Parse(DateFilter)));
            }

            return View(this._ticketService.GetAll());
        }

        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = _ticketService.Find(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,MovieTitle,MovieImage,MovieGenre,ValidDate,Price")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.Id = Guid.NewGuid();
                _ticketService.Create(ticket);
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = _ticketService.Find(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Id,MovieTitle,MovieGenre,ValidDate,MovieImage,Price")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(ticket);
            }

            try
            {
                _ticketService.Update(ticket);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(ticket.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete(Guid? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var ticket = _ticketService.Find(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _ticketService.Delete(id);
            return RedirectToAction(nameof(Index));
        }


        public IActionResult AddTicketToCart(Guid id)
        {
            var ShoppingCart = _ticketService.GetShoppingCartInfo(id); 
            return View(ShoppingCart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTicketToCart(AddToShoppingCartDTO model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = this._ticketService.AddToShoppingCart(model, userId);
            if (result)
            {
                return RedirectToAction("Index", "Tickets");
            }
            return View(model);
        }
        private bool TicketExists(Guid id)
        {
            return _ticketService.Find(id) != null;
        }

        public IActionResult Export()
        {
            var model = new ExportTIcket();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Export(ExportTIcket model)
        {
            string fileName = "Tickets.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            List<Ticket> result = _ticketService.GetAllTicketsWithGenre(model.Genre);

            try
            {
                using (var workBook = new XLWorkbook())
                {
                    IXLWorksheet worksheet = workBook.Worksheets.Add("Tickets");

                    worksheet.Cell(1, 1).Value = "Ticket Id";
                    worksheet.Cell(1, 2).Value = "Genre";
                    worksheet.Cell(1, 3).Value = "Title";
                    worksheet.Cell(1, 4).Value = "Valid Date";
                    worksheet.Cell(1, 5).Value = "Price";

                    for (int i = 1; i <= result.Count(); i++)
                    {
                        var item = result[i - 1];

                        worksheet.Cell(i + 1, 1).Value = item.Id.ToString();
                        worksheet.Cell(i + 1, 2).Value = item.MovieGenre.ToString();
                        worksheet.Cell(i + 1, 3).Value = item.MovieTitle.ToString();
                        worksheet.Cell(i + 1, 4).Value = item.ValidDate.ToString();
                        worksheet.Cell(i + 1, 5).Value = item.Price.ToString();
                    }

                    using (var stream = new MemoryStream())
                    {
                        workBook.SaveAs(stream);

                        var content = stream.ToArray();



                        //var fileContentResult = new FileContentResult(content, contentType)
                        //{
                        //FileDownloadName = fileName
                        //};
                        //return RedirectToAction("Index", "Home");
                        System.Console.WriteLine("HELLO");
                        return File(content, contentType, fileName);

                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Tickets");
            }
        }


    }
}
