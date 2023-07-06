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
    public class ShoppingCartService : IShoppingCartService
    {

        private readonly IRepository<TicketInOrder> _TicketInOrderRepository;
        private readonly IUserRepository _UserRepository;
        private readonly IRepository<ShoppingCart> _ShoppingCartRepository;
        private readonly IRepository<Order> _OrderRepository;
        private readonly IRepository<EmailMessage> _MailRepository;

        public ShoppingCartService(IRepository<ShoppingCart> shoppingCartRepository, IUserRepository userRepository, IRepository<EmailMessage> mailRepository, IRepository<Order> orderRepository, IRepository<TicketInOrder> ticketInOrderRepository)
        {
            _ShoppingCartRepository = shoppingCartRepository;
            _UserRepository = userRepository;
            _OrderRepository = orderRepository;
            _TicketInOrderRepository = ticketInOrderRepository;
            _MailRepository = mailRepository;
        }


        public bool DeleteTicket(string userId, Guid ticketId)
        {
            if (string.IsNullOrEmpty(userId) || ticketId == Guid.Empty)
                return false;

            var loggedInUser = _UserRepository.Get(userId);
            var userShoppingCart = loggedInUser?.ShoppingCart;

            if (userShoppingCart == null)
                return false;

            var itemToDelete = userShoppingCart.TicketInShoppingCart.FirstOrDefault(z => z.TicketId == ticketId);

            if (itemToDelete == null)
                return false;

            userShoppingCart.TicketInShoppingCart.Remove(itemToDelete);
            _ShoppingCartRepository.Update(userShoppingCart);

            return true;
        }

        public ShoppingCartDTO Find(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var loggedInUser = _UserRepository.Get(userId);
                var userCart = loggedInUser?.ShoppingCart;

                if (userCart != null)
                {
                    var allTicketPrices = userCart.TicketInShoppingCart
                        .Select(z => new
                        {
                            TicketPrice = z.Ticket.Price,
                            Quantity = z.Quantity
                        }).ToList();

                    double totalPrice = allTicketPrices.Sum(item => item.Quantity * item.TicketPrice);

                    var result = new ShoppingCartDTO
                    {
                        Tickets = userCart.TicketInShoppingCart.ToList(),
                        TotalPrice = totalPrice
                    };

                    return result;
                }
            }

            return new ShoppingCartDTO();

        }

        public bool Order(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var loggedInUser = _UserRepository.Get(userId);
                var userCard = loggedInUser.ShoppingCart;

                EmailMessage mail = new EmailMessage
                {
                    MailTo = loggedInUser.Email,
                    Subject = "The order has been created successfully!",
                    Status = false
                };
                Order order = new Order
                {
                    Id = Guid.NewGuid(),
                    User = loggedInUser,
                    UserId = userId
                };

                _OrderRepository.Insert(order);
                List<TicketInOrder> ticketInOrders = new List<TicketInOrder>();

                var result = userCard.TicketInShoppingCart.Select(z => new TicketInOrder
                {
                    Id = Guid.NewGuid(),
                    TicketId = z.Ticket.Id,
                    Ticket = z.Ticket,
                    OrderId = order.Id,
                    Order = order,
                    Quantity = z.Quantity
                }).ToList();

                StringBuilder sb = new StringBuilder();

                var totalPrice = result.Sum(item => item.Quantity * item.Ticket.Price);

                sb.AppendLine("Your order is completed. The order contains: ");

                for (int i = 0; i < result.Count; i++)
                {
                    var currentItem = result[i];
                    sb.AppendLine($"{i + 1}. {currentItem.Ticket.MovieTitle} with a quantity of: {currentItem.Quantity} and a price of: ${currentItem.Ticket.Price}");
                }

                sb.AppendLine($"Total price for your order: ${totalPrice}");

                mail.Content = sb.ToString();


                ticketInOrders.AddRange(result);

                foreach (var item in ticketInOrders)
                {
                    _TicketInOrderRepository.Insert(item);
                }

                loggedInUser.ShoppingCart.TicketInShoppingCart.Clear();

                _UserRepository.Update(loggedInUser);
                _MailRepository.Insert(mail);
                return true;
            }
            return false;
        }
    }
}
