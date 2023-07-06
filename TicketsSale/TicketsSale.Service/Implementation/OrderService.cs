using System;
using System.Collections.Generic;
using System.Text;
using TicketsSale.Domain;
using TicketsSale.Domain.DomainModels;
using TicketsSale.Repository.Interface;
using TicketsSale.Service.Interface;

namespace TicketsSale.Service.Implementation
{
    public class OrderService : IOrderService
    {


        private readonly IOrderRepository _OrderRepository;


        public OrderService(IOrderRepository OrderRepository)
        {
            _OrderRepository = OrderRepository;
        }

        public Order Get(BaseEntity model)
        {
            return this._OrderRepository.Get(model);
        }

        public List<Order> GetAll()
        {
            return this._OrderRepository.GetAll();
        }

        public List<Order> GetAll(string user_id)
        {
            return this._OrderRepository.GetAll(user_id);
        }
    }
}
