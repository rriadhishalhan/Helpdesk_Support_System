using API.Base;
using API.Models;
using API.Repository.Data;
using API.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
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
            _configuration = configuration;
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

        [HttpPost("login")]
        public ActionResult Login(LoginVM loginVM)
        {
            try
            {
                var loginResult = employeeRepository.Login(loginVM);

                if (loginResult == -1)
                {
                    //return BadRequest("Email is not exist");
                    return NotFound(new { status = HttpStatusCode.NotFound, token = (object)null, message = "Email is not exist" });

                }

                if (loginResult == -2)
                {
                    //return BadRequest("Password is incorrect");
                    return NotFound(new { status = HttpStatusCode.NotFound, token = (object)null, message = "Password is incorrect" });

                }

                string employeeId = employeeRepository.GetEmployeeId(loginVM.Email);
                string employeeFullName = employeeRepository.GetEmployeeFullName(loginVM.Email);
                string employeeRoleName = employeeRepository.GetEmployeeRoleName(loginVM.Email);
                var claims = new List<Claim>
                {
                    new Claim("Email", loginVM.Email),
                    new Claim("Id",employeeId),
                    new Claim("Name", employeeFullName),
                    new Claim("roles", employeeRoleName),
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
