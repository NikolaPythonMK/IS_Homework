using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TicketsSale.Domain.Identity;
using TicketsSale.Repository.Interface;

namespace TicketsSale.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<TicketsSaleApplicationUser> entities;


        public UserRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<TicketsSaleApplicationUser>();
        }



        public void Delete(TicketsSaleApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Remove(entity);
            context.SaveChanges();
        }

        public TicketsSaleApplicationUser Get(string id)
        {
                return entities
               .Include(z => z.ShoppingCart)
               .Include("ShoppingCart.TicketInShoppingCart")
               .Include("ShoppingCart.TicketInShoppingCart.Ticket")
               .SingleOrDefault(s => s.Id == id);
        }

        public IEnumerable<TicketsSaleApplicationUser> GetAll()
        {
            return entities.AsEnumerable();
        }

        public void Insert(TicketsSaleApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Add(entity);
            context.SaveChanges();
        }

        public void Update(TicketsSaleApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Update(entity);
            context.SaveChanges();
        }
    }
}
