﻿using HomeBankingMindHub.Models;
using System.Collections.Generic;

namespace HomeBankingMindHub.Repositories
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAllAccounts();
        Account FindById(long id);
    }
}
