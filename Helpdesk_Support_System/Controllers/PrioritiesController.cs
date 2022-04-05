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
    public class PrioritiesController : BaseController<Priority, PriorityRepository, int>
    {
        public PrioritiesController(PriorityRepository priorityRepository) : base(priorityRepository)
        {

        }

    }
}
