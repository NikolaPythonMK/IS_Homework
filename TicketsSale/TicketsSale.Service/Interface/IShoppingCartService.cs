using System;
using System.Collections.Generic;
using System.Text;
using TicketsSale.Domain.DomainModels;
using TicketsSale.Domain.DTO;

namespace TicketsSale.Service.Interface
{
    public interface IShoppingCartService
    {
        ShoppingCartDTO Find(string userId);
        bool DeleteTicket(string userId, Guid ticketId);
        bool Order(string userId);
    }
}
