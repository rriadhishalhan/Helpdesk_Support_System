
using API.ViewModel;
using Client.Base;
using Client.Repositories.Data;
using Client.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Controllers
{
    public class LoginCustomerController : BaseController<LoginVM,LoginCustomerRepository,string>
    {
        private readonly LoginCustomerRepository repository;
        public LoginCustomerController(LoginCustomerRepository repository): base(repository)
        {
            this.repository = repository;
        }

        [HttpPost("LoginCustomer/")]
        public async Task<IActionResult> Auth(LoginVM login)
        {
            var jwtToken = await repository.Auth(login);
            var token = jwtToken.token;


            if (token == null)
            {
                return RedirectToAction("index");
            }

            HttpContext.Session.SetString("JWToken", token);

            //JWTokenPayload objPayload = new JWTokenPayload();
            //objPayload.id = "TEST01";
            //HttpContext.Session.SetString("Name", jwtHandler.GetName(token));
            //HttpContext.Session.SetString("ProfilePicture", "assets/img/theme/user.png");

            // GET CLAIMS
            var claimsId = GetClaim(token,"Id");
            var claimsName = GetClaim(token,"Name");
            var claimsEmail = GetClaim(token, "Email");
            var claimsRole = GetClaim(token, "roles");
            // SET SESSION
            HttpContext.Session.SetString("Id", claimsId);
            HttpContext.Session.SetString("Name", claimsName);
            HttpContext.Session.SetString("Role", claimsRole);
            HttpContext.Session.SetString("Email", claimsEmail);



            //TempData["CustomerId"] = claimsId;
            //TempData["CustomerName"] = claimsName;
            //TempData["CustomerEmail"] = claimsEmail;
            //TempData["CustomerRole"] = claimsRole;
            //new RedirectResult(@"~\DashboardCustomer");


            return RedirectToAction("index", "DashboardCustomer");
        }

        public string GetClaim(string token, string claimType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            var stringClaimValue = securityToken.Claims.First(claim => claim.Type == claimType).Value;
            return stringClaimValue;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
