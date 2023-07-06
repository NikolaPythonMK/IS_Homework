using System;
using System.Collections.Generic;
using System.Text;
using TicketsSale.Domain.DomainModels;

namespace TicketsSale.Domain.DTO
{
    public class AddToShoppingCartDTO
    {
        public Guid TicketId { get; set; }
        public Ticket Ticket { get; set; }
        public int Quantity { get; set; }
    }
}
