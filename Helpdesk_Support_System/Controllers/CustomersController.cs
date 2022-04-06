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
    public class CustomersController : BaseController<Customer, CustomerRepository, string>
    {
        private readonly CustomerRepository customerRepository;
        public IConfiguration _configuration;

        public CustomersController(CustomerRepository customerRepository , IConfiguration configuration) : base(customerRepository)
        {
            this.customerRepository = customerRepository;
            this._configuration = configuration;
        }

        [HttpPost("register")]
        public ActionResult Register(RegisterCustomerVM registerCustomerVM)
        {
            try
            {
                var dataRegister = customerRepository.RegisterCustomer(registerCustomerVM);
                if (dataRegister > 0)
                {
                    return Ok("Akun berhasil ditambahkan");
                }
                else if (dataRegister == -1)
                {
                    return BadRequest("Email sudah terdaftar");
                }
                else
                {
                    return BadRequest("Gagal melakukan Register");
                }
            }
            catch (Exception e)
            {
                //return BadRequest(e);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "REGISTER Server Error");
            }
        }

    }
}
