using System;
using System.Collections.Generic;
using System.Text;
using TicketsSale.Domain;
using TicketsSale.Domain.DomainModels;

namespace TicketsSale.Service.Interface
{
    public interface IOrderService
    {
        public List<Order> GetAll();
        public List<Order> GetAll(string user_id);
        public Order Get(BaseEntity model);
    }
}
