using System;
using System.Collections.Generic;
using System.Text;
using TicketsSale.Domain.Identity;

namespace TicketsSale.Domain.DTO
{
    public class UserToRole
    {
        public string Email { get; set; }
        public string SelectedRole { get; set; }


        public List<string> UserRoles { get; set; }
        public ICollection<TicketsSaleApplicationUser> UserEmails { get; set; }
       

        public UserToRole()
        {
            this.UserRoles = new List<string>();
            this.UserEmails = new List<TicketsSaleApplicationUser>();
        }
    }
}
