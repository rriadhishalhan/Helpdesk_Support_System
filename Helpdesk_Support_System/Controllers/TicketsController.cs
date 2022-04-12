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

                if (result == -1)
                {
                    return NotFound("Customer Not Found");
                }

                if (result == -2)
                {
                    return NotFound("Category Not Found");
                }

                if (result == -3)
                {
                    return BadRequest("Issue cant be null");
                }

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

        [HttpGet("MasterTicket/{status}")]
        public ActionResult MasterTicket(string status)
        {

            try
            {
                var dataMaster = ticketRepository.MasterTicket(status);
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

        [HttpPut("setPriority")]
        public ActionResult SetPriority(SetPriorityVM setPriorityVM)
        {
            try
            {
                int result = ticketRepository.SetPriority(setPriorityVM);

                if (result == -1)
                {
                    return NotFound("Ticket not Found");
                }

                if (result == -2)
                {
                    return NotFound("Priority not Found");
                }

                if (result == 0)
                {
                    return BadRequest("Set Priority Ticket Failed");
                }

                return Ok("Set Priority Ticket Success");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  "SET PRIORITY TICKET Server Error");
            }
        }

        [HttpPost("forward")]
        public ActionResult Forward(ForwardTicketVM forwardTicketVM)
        {
            try
            {
                int result = ticketRepository.Forward(forwardTicketVM);

                if (result == -1)
                {
                    return NotFound("Ticket not Found");
                }

                if (result == -2)
                {
                    return NotFound("No Employee is Vacant NOW");
                }

                if (result == 0)
                {
                    return BadRequest("Forward Ticket Failed");
                }

                return Ok("Forward Ticket Success");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  "FORWARD TICKET Server Error");
            }
        }

        [HttpPost("open")]
        public ActionResult Open(OpenTicketVM openTicketVM)
        {
            try
            {
                int result = ticketRepository.Open(openTicketVM);

                if (result == -1)
                {
                    return NotFound("Ticket not Found");
                }

                if (result == -2)
                {
                    return NotFound("Employee not Found");
                }

                if (result == 0)
                {
                    return BadRequest("Open Ticket Failed");
                }

                return Ok("Open Ticket Success");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  "OPEN TICKET Server Error");
            }
        }

        [HttpPut("respond")]
        public ActionResult Respond(RespondTicketVM respondTicketVM)
        {
            try
            {
                int result = ticketRepository.Respond(respondTicketVM);

                if (result == -1)
                {
                    return NotFound("Ticket not Found");
                }

                if (result == -2)
                {
                    return NotFound("Employee not Found");
                }

                if (result == -3)
                {
                    return BadRequest("Solution cant be null");
                }

                return Ok("Respond Ticket Success");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  "RESPOND TICKET Server Error");
            }
        }


    }
}
