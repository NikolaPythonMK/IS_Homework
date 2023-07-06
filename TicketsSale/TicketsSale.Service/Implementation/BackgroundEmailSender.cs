using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketsSale.Domain.DomainModels;
using TicketsSale.Repository.Interface;
using TicketsSale.Service.Interface;

namespace TicketsSale.Service.Implementation
{
    public class BackgroundEmailSender : IBackgroundEmailSender
    {
        private readonly IEmailService _emailService;
        private readonly IRepository<EmailMessage> _mailRepository;

        public BackgroundEmailSender(IEmailService emailService, IRepository<EmailMessage> mailRepository)
        {
            _emailService = emailService;
            _mailRepository = mailRepository;
        }

        public async Task DoWork()
        {
            await _emailService.SendEmailAsync(_mailRepository.GetAll().Where(z => !z.Status).ToList());
            var mails = _mailRepository.GetAll().Where(z => !z.Status).ToList();

            foreach (EmailMessage mail in mails)
            {
                mail.Status = true;

                _mailRepository.Update(mail);
            }
        }
    }
}
