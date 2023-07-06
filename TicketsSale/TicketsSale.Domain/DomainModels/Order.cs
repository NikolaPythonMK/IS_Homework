using System;
using System.Collections.Generic;
using System.Text;
using TicketsSale.Domain.Identity;
using TicketsSale.Domain.Relations;

namespace TicketsSale.Domain.DomainModels
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; }
        public TicketsSaleApplicationUser User { get; set; }

        public virtual ICollection<TicketInOrder> TicketInOrder { get; set; }
    }
}
