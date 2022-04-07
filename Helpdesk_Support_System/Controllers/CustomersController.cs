using API.Base;
using API.Models;
using API.Repository.Data;
using API.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : BaseController<Customer, CustomerRepository, string>
    {
        private readonly CustomerRepository customerRepository;
        public IConfiguration _configuration;

        public CustomersController(CustomerRepository customerRepository, IConfiguration configuration) : base(customerRepository)
        {
            this.customerRepository = customerRepository;
            _configuration = configuration;
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
            catch (Exception)
            {
                //return BadRequest(e);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "REGISTER Server Error");
            }
        }

        [HttpPost("login")]
        public ActionResult Login(LoginVM loginVM)
        {
            try
            {
                var loginResult = customerRepository.Login(loginVM);

                if (loginResult == -1)
                {
                    return BadRequest("Email is not exist");
                }

                if (loginResult == -2)
                {
                    return BadRequest("Password is incorrect");
                }

                var claims = new List<Claim>
                {
                    new Claim("Email", loginVM.Email),
                    new Claim("roles", "Customer")
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(10),
                    signingCredentials: signIn);
                var idtoken = new JwtSecurityTokenHandler().WriteToken(token);
                claims.Add(new Claim("TokenSecurity", idtoken.ToString()));
                return Ok(new { status = 200, token = idtoken, message = "Login Success" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "LOGIN Server Error");
            }
        }

    }
}
