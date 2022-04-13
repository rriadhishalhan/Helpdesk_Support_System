using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.ViewModel
{
    public class ForwardMessageVM
    {
        public string Employee_Id { get; set; }
        public string Employee_Name { get; set; }
        public string Employee_Position { get; set; }
        public int Status_Code { get; set; }
        public string Error_Message { get; set; }
    }
}
