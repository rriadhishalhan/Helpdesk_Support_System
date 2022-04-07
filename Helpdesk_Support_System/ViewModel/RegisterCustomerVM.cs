using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.ViewModel
{
    public class RegisterCustomerVM
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string phone_number { get; set; }
    }
}
