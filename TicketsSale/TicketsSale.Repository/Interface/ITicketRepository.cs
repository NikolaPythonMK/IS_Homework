using System;
using System.Collections.Generic;
using System.Text;
using TicketsSale.Domain.DomainModels;

namespace TicketsSale.Repository.Interface
{
    public interface ITicketRepository
    {
        List<Ticket> GetAllTicketsAsync();
        List<Ticket> GetTicketsByDateAsync(DateTime date);
        Ticket GetTicketByIdAsync(int id);
        List<Ticket> GetAllTicketsFromGenre(string genre);
    }
}
