using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TicketsSale.Domain;
using TicketsSale.Domain.DomainModels;
using TicketsSale.Repository.Interface;

namespace TicketsSale.Repository.Implementation
{
    public class OrderRepository : IOrderRepository
    {

        private readonly ApplicationDbContext context;
        private DbSet<Order> entities;
        string errorMessage = string.Empty;

        public OrderRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<Order>();
        }


        public Order Get(BaseEntity model)
        {
            return entities
               .Include(z => z.User)
               .Include(z => z.TicketInOrder)
               .Include("TicketInOrder.Ticket")
               .SingleOrDefaultAsync(z => z.Id == model.Id).Result;
        }

        public List<Order> GetAll()
        {
              return entities
             .Include(z => z.User)
             .Include(z => z.TicketInOrder)
             .Include("TicketInOrder.Ticket")
             .ToListAsync().Result;
        }

        public List<Order> GetAll(string userId)
        {
            return entities
                .Include(z => z.User)
                .Include(z => z.TicketInOrder)
                .Include("TicketInOrder.Ticket")
                .ToListAsync()
                .Result.FindAll(z => z.UserId == userId);
        }
    }
}
