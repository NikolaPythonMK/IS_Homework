using System;
using System.Collections.Generic;
using System.Text;
using TicketsSale.Domain;
using TicketsSale.Domain.DomainModels;

namespace TicketsSale.Repository.Interface
{
    public interface IOrderRepository
    {
        public List<Order> GetAll();
        public List<Order> GetAll(string userId);
        public Order Get(BaseEntity model);
    }
}
