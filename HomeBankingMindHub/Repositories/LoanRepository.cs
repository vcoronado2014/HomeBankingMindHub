﻿using HomeBankingMindHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace HomeBankingMindHub.Repositories
{
    public class LoanRepository : RepositoryBase<Loan>, ILoanRepository
    {
        public LoanRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public Loan FindById(long id)
        {
            return FindByCondition(loan => loan.Id == id).FirstOrDefault();
        }

        public IEnumerable<Loan> GetAll()
        {
            return FindAll().ToList();
        }
    
    }
}
