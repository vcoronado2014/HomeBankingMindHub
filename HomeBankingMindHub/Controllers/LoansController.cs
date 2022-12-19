using HomeBankingMindHub.dtos;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ILoanRepository _loanRepository;
        private IClientLoanRepository _clientLoanRepository;
        private ITransactionRepository _transactionRepository;

        public LoansController(IClientRepository clientRepository, 
            IAccountRepository accountRepository, 
            ILoanRepository loanRepository, 
            IClientLoanRepository clientLoanRepository, 
            ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _loanRepository = loanRepository;
            _clientLoanRepository = clientLoanRepository;
            _transactionRepository = transactionRepository;
        }

        //metodo get traer todo
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var loans = _loanRepository.GetAll();

                var loansDTO = new List<LoanDTO>();

                foreach (Loan loan in loans)
                {
                    var newLoanDto = new LoanDTO
                    {
                        Id = loan.Id,
                        MaxAmount = loan.MaxAmount,
                        Name = loan.Name,
                        Payments = loan.Payments,
                    };

                    
                    loansDTO.Add(newLoanDto);
                }


                return Ok(loansDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] LoanApplicationDTO loanApplicationDTO)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                //buscamos loan
                var loan = _loanRepository.FindById(loanApplicationDTO.LoanId);

                if (loan == null)
                {
                    return Forbid("Crédito no existe");
                }

                //seguimos con las validaciones
                if (loanApplicationDTO.Amount == 0 || loanApplicationDTO.Amount > loan.MaxAmount)
                {
                    return Forbid("No hay monto o el monto excede el máximo del préstamo");
                }

                if (loanApplicationDTO.Payments == string.Empty)
                {
                    return Forbid("No hay cantidad de pagos");
                }

                //ahora buscamos account
                Account account = _accountRepository.FinByNumber(loanApplicationDTO.ToAccountNumber);
                if (account == null)
                {
                    return Forbid("Cuenta de destino no existe");
                }
                //validamos que la cuenta de destino pertenezca al cliente autenticado
                Account accountClient = client.Accounts.FirstOrDefault(c => c.Number == account.Number);
                if (accountClient == null)
                {
                    return Forbid("La cuenta de destino no pertenece al cliente autenticado");
                }

                //ahora empezamos a procesar
                //insertamos client loan
                _clientLoanRepository.Save(new ClientLoan
                {
                    Amount = (loanApplicationDTO.Amount + (loanApplicationDTO.Amount * 0.2)),
                    Payments = loanApplicationDTO.Payments,
                    ClientId = client.Id,
                    LoanId= loanApplicationDTO.LoanId,
                });

                //ahora transaccion
                _transactionRepository.Save(new Transaction
                {
                    Type = TransactionType.CREDIT.ToString(),
                    Amount = loanApplicationDTO.Amount,
                    Description = loan.Name + " loan approved",
                    Date = DateTime.Now,
                    AccountId= account.Id,
                });

                //ahora actualizamos el balance de account
                account.Balance = account.Balance + loanApplicationDTO.Amount;
                _accountRepository.Save(account);

                //y retornamos
                return Created("Préstamo aprobado", account);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
