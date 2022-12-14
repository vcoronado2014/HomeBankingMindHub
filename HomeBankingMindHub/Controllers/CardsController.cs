using HomeBankingMindHub.dtos;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/clients")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private ICardRepository _cardRepository;

        public CardsController(IClientRepository clientRepository, ICardRepository cardRepository)
        {
            _clientRepository = clientRepository;
            _cardRepository = cardRepository;
        }

        [HttpGet("current/cards")]
        public IActionResult GetCards()
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

                var cards = _cardRepository.FindByClienId(client.Id);

                var cardsDTO = new List<CardDTO>();

                foreach (var card in cards)
                {
                    var newCarDto = new CardDTO
                    {
                        Id = card.Id,
                        CardHolder = card.CardHolder,
                        Color = card.Color,
                        Cvv = card.Cvv,
                        FromDate = card.FromDate,
                        Number = card.Number,
                        ThruDate = card.ThruDate,
                        Type = card.Type,
                    }; 

                    cardsDTO.Add(newCarDto);
                }

                return Ok(cardsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/cards")]
        public IActionResult CreateCard([FromBody] Card card)
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

                //validamos que el cliente tenga máximo 3 tarjetas creadas
                if (client.Cards.ToList().FindAll(card => card.Type.ToUpper() == card.Type.ToUpper()).Count() >= 3)
                {
                    return Forbid("You already have 3 " + card.Type + " cards");
                }

                //bloques de la card
                Random rnd = new Random();
                string primerBloque = rnd.Next(1000,3999).ToString();
                string segundoBloque = rnd.Next(4000, 5999).ToString();
                string tercerBloque = rnd.Next(6000, 7999).ToString();
                string cuartoBloque = rnd.Next(8000, 9999).ToString();

                string cardNumber = primerBloque +"-"+segundoBloque+"-"+tercerBloque+"-"+cuartoBloque;
                int cvv = rnd.Next(100, 999);

                //asignamos los demas valores a la card
                card.ClientId = client.Id;
                card.Number = cardNumber;
                card.ThruDate = DateTime.Now.AddYears(5);
                card.FromDate = DateTime.Now;
                card.Color = card.Color.ToUpper();
                card.CardHolder = client.FirstName + " " + client.LastName;
                card.Cvv = cvv;
                card.Type= card.Type.ToUpper();

                //guardamos en el repositorio
                _cardRepository.Save(card);

                return Ok(card);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
