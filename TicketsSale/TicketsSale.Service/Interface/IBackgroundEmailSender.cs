﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TicketsSale.Service.Interface
{
    public interface IBackgroundEmailSender
    {
        Task DoWork();
    }
}
