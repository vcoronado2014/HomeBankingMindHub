using HomeBankingMindHub.dtos;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private IAccountRepository _accountRepository;

        public AccountsController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var accounts = _accountRepository.GetAllAccounts();

                var accountsDTO = new List<AccountDTO>();

                foreach (Account account in accounts)
                {
                    var newAccountDTO = new AccountDTO
                    {
                        Id= account.Id,
                        Balance= account.Balance,
                        CreationDate = account.CreationDate,
                        Number= account.Number,
                        Transactions = account.Transactions.Select(tr => new TransactionDTO
                        {
                            Id= tr.Id,
                            Amount= tr.Amount,
                            Date = tr.Date,
                            Description= tr.Description,
                            Type = tr.Type

                        }).ToList(),

                    };

                    accountsDTO.Add(newAccountDTO);
                }
                return Ok(accountsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                var account = _accountRepository.FindById(id);
                if (account == null)
                {
                    return Forbid();
                }

                var accountDTO = new AccountDTO
                {
                    Id= account.Id,
                    Balance= account.Balance,
                    CreationDate = account.CreationDate,
                    Number= account.Number,
                    Transactions = account.Transactions.Select(tr => new TransactionDTO
                    {
                        Id = tr.Id,
                        Amount = tr.Amount,
                        Date = tr.Date,
                        Description = tr.Description,
                        Type = tr.Type

                    }).ToList(),
                };

                return Ok(accountDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
