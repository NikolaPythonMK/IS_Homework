using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TicketsSale.Domain.Relations;

namespace TicketsSale.Domain.DomainModels
{
    public class Ticket : BaseEntity
    {
        [Required]
        public string MovieTitle { get; set; }
        [Required]
        public string MovieGenre { get; set; }
        [Required]
        public DateTime ValidDate { get; set; }
        public string MovieImage { get; set; }
        public int Price { get; set; }

        public virtual ICollection<TicketInShoppingCart> TicketInShoppingCart { get; set; }
        public virtual ICollection<TicketInOrder> TicketInOrder { get; set; }
    }
}
