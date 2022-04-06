using API.Base;
using API.Models;
using API.Repository.Data;
using API.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : BaseController<Employee, EmployeeRepository, string>
    {
        private readonly EmployeeRepository employeeRepository;
        public IConfiguration _configuration;
        public EmployeesController(EmployeeRepository employeeRepository, IConfiguration configuration) : base(employeeRepository)
        {
            this.employeeRepository = employeeRepository;
            this._configuration = configuration;
        }


        //[HttpPost("register")]
        //public ActionResult Register(RegisterEmployeeVM registerEmployeeVM)
        //{
        //    try
        //    {
        //        int registerResult = employeeRepository.Register(registerEmployeeVM);

        //        if (registerResult == -1)
        //        {
        //            return BadRequest("Email already used");
        //        }

        //        if (registerResult == -2)
        //        {
        //            return BadRequest("Phone already used");
        //        }

        //        return Ok("Register success");
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e);
        //        //return StatusCode(StatusCodes.Status500InternalServerError, "Registration Failed");

        //    }
        //}


    }
}
