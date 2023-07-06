using System;
using System.Collections.Generic;
using System.Text;
using TicketsSale.Domain.DomainModels;
using TicketsSale.Domain.DTO;

namespace TicketsSale.Service.Interface
{
    public interface ITicketService
    {
        List<Ticket> GetAll();
        List<Ticket> GetAllTicketsFromDate(DateTime date);
        List<Ticket> GetAllTicketsWithGenre(string genre);
        AddToShoppingCartDTO GetShoppingCartInfo(Guid? id);
        Ticket Find(Guid? id);
        void Create(Ticket p);
        void Update(Ticket p);
        void Delete(Guid id);
        bool AddToShoppingCart(AddToShoppingCartDTO dtoItem, string userID);
    }
}
