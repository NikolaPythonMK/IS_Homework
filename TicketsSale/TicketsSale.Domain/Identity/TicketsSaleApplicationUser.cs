using System;
using System.Collections.Generic;
using System.Text;
using TicketsSale.Domain.DomainModels;
using Microsoft.AspNetCore.Identity;

namespace TicketsSale.Domain.Identity
{
    public class TicketsSaleApplicationUser : IdentityUser
    {
        public virtual ShoppingCart ShoppingCart { get; set; }
    }
}
