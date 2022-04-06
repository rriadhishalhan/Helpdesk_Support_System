using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.ViewModel
{
    public class CreateTicketVM
    {
        public string Customer_Id { get; set; }
        public int Category_Id { get; set; }
        public string Issue { get; set; }
    }
}
