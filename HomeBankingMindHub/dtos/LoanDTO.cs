﻿using HomeBankingMindHub.Models;
using System.Collections.Generic;

namespace HomeBankingMindHub.dtos
{
    public class LoanDTO
    {

        public long Id { get; set; }
        public string Name { get; set; }
        public double MaxAmount { get; set; }
        public string Payments { get; set; }
    }
}
