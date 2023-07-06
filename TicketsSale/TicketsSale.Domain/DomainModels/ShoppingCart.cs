using System;
using System.Collections.Generic;
using System.Text;
using TicketsSale.Domain.Identity;
using TicketsSale.Domain.Relations;

namespace TicketsSale.Domain.DomainModels
{
    public class ShoppingCart : BaseEntity
    {
        public string UserId { get; set; }
        public virtual TicketsSaleApplicationUser User { get; set; }
        public virtual ICollection<TicketInShoppingCart> TicketInShoppingCart { get; set; }
    }
}
