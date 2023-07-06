using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TicketsSale.Domain.DomainModels;
using TicketsSale.Domain.DTO;
using TicketsSale.Domain.Relations;
using TicketsSale.Repository.Interface;
using TicketsSale.Service.Interface;

namespace TicketsSale.Service.Implementation
{
    public class TicketService : ITicketService
    {

        private readonly IUserRepository _UserRepository;
        private readonly IRepository<Ticket> _Repository_Ticket;
        private readonly ITicketRepository _TicketRepository;
        private readonly IRepository<TicketInShoppingCart> _TicketInShoppingCartRepository;


        public TicketService(IRepository<Ticket> Repository_Ticket, IRepository<TicketInShoppingCart> TicketInShoppingCartRepository, IUserRepository UserRepository, ITicketRepository TicketRepository)
        {
            _Repository_Ticket = Repository_Ticket;
            _UserRepository = UserRepository;
            _TicketInShoppingCartRepository = TicketInShoppingCartRepository;
            _TicketRepository = TicketRepository;
        }

        public bool AddToShoppingCart(AddToShoppingCartDTO dtoItem, string userID)
        {
            var user = _UserRepository.Get(userID);
            var userShoppingCart = user?.ShoppingCart;

            if (dtoItem.TicketId == null || userShoppingCart == null)
                return false;


            var ticket = Find(dtoItem.TicketId);
            if (ticket == null)
                return false;


            var itemToAdd = new TicketInShoppingCart
            {
                Id = Guid.NewGuid(),
                Ticket = ticket,
                TicketId = ticket.Id,
                ShoppingCart = userShoppingCart,
                ShoppingCartId = userShoppingCart.Id,
                Quantity = dtoItem.Quantity
            };


            var existing = userShoppingCart.TicketInShoppingCart
                .FirstOrDefault(z => z.ShoppingCartId == userShoppingCart.Id && z.TicketId == itemToAdd.TicketId);

            if (existing != null)
            {
                existing.Quantity += itemToAdd.Quantity;
                _TicketInShoppingCartRepository.Update(existing);
            }
            else
            {
                _TicketInShoppingCartRepository.Insert(itemToAdd);
            }

            return true;

        }

        public void Create(Ticket p)
        {
            _Repository_Ticket.Insert(p);
        }

        public void Delete(Guid id)
        {
            Ticket ticket = this.Find(id);
           _Repository_Ticket.Delete(ticket);
        }

        public Ticket Find(Guid? id)
        {
            return _Repository_Ticket.Get(id);
        }

        public List<Ticket> GetAll()
        {
            return _Repository_Ticket.GetAll().ToList();
        }

        public List<Ticket> GetAllTicketsFromDate(DateTime date)
        {
            return _TicketRepository.GetTicketsByDateAsync(date);
        }

        public List<Ticket> GetAllTicketsWithGenre(string genre)
        {
            return _TicketRepository.GetAllTicketsFromGenre(genre);
        }

        public AddToShoppingCartDTO GetShoppingCartInfo(Guid? id)
        {
            Ticket ticket = Find(id);
            AddToShoppingCartDTO Model = new AddToShoppingCartDTO
            {
                Ticket = ticket,
                TicketId = ticket.Id,
                Quantity = 1 // default
            };

            return Model;
        }

        public void Update(Ticket p)
        {
            _Repository_Ticket.Update(p);
        }
    }
}
