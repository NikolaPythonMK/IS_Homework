using System;
using System.Collections.Generic;
using System.Text;
using TicketsSale.Domain.Identity;

namespace TicketsSale.Repository.Interface
{
    public interface IUserRepository
    {
        IEnumerable<TicketsSaleApplicationUser> GetAll();
        TicketsSaleApplicationUser Get(string id);
        void Insert(TicketsSaleApplicationUser entity);
        void Update(TicketsSaleApplicationUser entity);
        void Delete(TicketsSaleApplicationUser entity);
    }
}
