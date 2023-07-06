using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketsSale.Domain.DomainModels;
using TicketsSale.Repository.Interface;

namespace TicketsSale.Repository.Implementation
{
    public class TicketRepository : ITicketRepository
    {

        private readonly ApplicationDbContext _dbContext;
        private DbSet<Ticket> _entities;

        public TicketRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _entities = _dbContext.Set<Ticket>();
        }

        public List<Ticket> GetAllTicketsAsync()
        {
            return _entities.ToList();
        }

        public List<Ticket> GetAllTicketsFromGenre(string genre)
        {
            return _entities.Where(t => t.MovieGenre == genre).ToList();
        }

        public Ticket GetTicketByIdAsync(int id)
        {
            return _entities.Find(id);
        }

        public List<Ticket> GetTicketsByDateAsync(DateTime date)
        {
            return _entities.Where(t => t.ValidDate.Date == date.Date).ToList();
        }
    }
}
