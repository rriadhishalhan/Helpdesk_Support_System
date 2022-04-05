using API.Base;
using API.Models;
using API.Repository.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketHistoriesController : BaseController<TicketHistory, TicketHistoryRepository, int>
    {
        public TicketHistoriesController(TicketHistoryRepository ticketHistoryRepository) : base(ticketHistoryRepository)
        {

        }

    }
}
