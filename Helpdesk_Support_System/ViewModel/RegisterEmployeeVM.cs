using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.ViewModel
{
    public class RegisterEmployeeVM
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public string Phone_number { get; set; }
        public int Position_id { get; set; }
    }
}
