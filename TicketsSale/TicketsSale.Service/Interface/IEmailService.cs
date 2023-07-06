using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TicketsSale.Domain.DomainModels;

namespace TicketsSale.Service.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(List<EmailMessage> allMails);
    }
}
