using API.Base;
using API.Models;
using API.Repository.Data;
using API.ViewModel;
using Microsoft.AspNetCore.Http;
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

        [HttpGet("MasterEmployeeData")]
        public ActionResult MasterEmployeeData()
        {
            try
            {
                var dataMaster = employeeRepository.MasterEmployee();
                if (dataMaster.Count != 0)
                {
                    return Ok(dataMaster);
                }
                else
                {
                    return NotFound("Tidak ada Data Master");
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   "MASTER DATA Server Error");
            }
        }

        [HttpPost("register")]
        public ActionResult Register(RegisterEmployeeVM registerEmployeeVM)
        {
            try
            {
                int registerResult = employeeRepository.Register(registerEmployeeVM);

                if (registerResult > 0)
                {
                    return Ok("Akun berhasil ditambahkan");
                }
                else if (registerResult == -1)
                {
                    return BadRequest("Email sudah terdaftar");
                }
                else
                {
                    return BadRequest("Gagal melakukan Register");
                }
            }
            catch (Exception)
            {
                //return BadRequest(e);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "REGISTER Server Error");
            }
        }


    }
}
