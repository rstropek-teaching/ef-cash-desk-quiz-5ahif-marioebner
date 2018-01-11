﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CashDesk.Models
{
    class DepositStatistics : IDepositStatistics
    {
        public IMember Member { get; set; }

        public int Year { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
