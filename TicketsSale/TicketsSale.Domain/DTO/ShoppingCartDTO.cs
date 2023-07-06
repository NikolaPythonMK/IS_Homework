using System;
using System.Collections.Generic;
using System.Text;
using TicketsSale.Domain.Relations;

namespace TicketsSale.Domain.DTO
{
    public class ShoppingCartDTO
    {
        public List<TicketInShoppingCart> Tickets { get; set; }
        public double TotalPrice { get; set; }
    }
}
