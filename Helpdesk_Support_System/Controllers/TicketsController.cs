using API.Base;
using API.Models;
using API.Repository.Data;
using API.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : BaseController<Ticket, TicketRepository, string>
    {
        private readonly TicketRepository ticketRepository;
        public TicketsController(TicketRepository ticketRepository) : base(ticketRepository)
        {
            this.ticketRepository = ticketRepository;
        }

        [HttpPost("createTicket")]
        public ActionResult CreateTicket(CreateTicketVM createTicketVM)
        {
            try
            {
                int result = ticketRepository.CreateTicket(createTicketVM);

                return Ok("Create Ticket Success");
            } catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Create Ticket Server Error");
            }
        }

        [HttpGet("MasterTicket")]
        public ActionResult MasterTicket()
        {
            
            try
            {
                var dataMaster = ticketRepository.MasterTicket();
                if (dataMaster.Count != 0)
                {
                    return Ok(dataMaster);
                }
                else
                {
                    return NotFound("Tidak ada data master Ticket");
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  "MASTER DATA TICKET Server Error");
            }
        }

    }
}
