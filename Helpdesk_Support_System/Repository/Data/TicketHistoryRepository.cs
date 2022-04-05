using API.Context;
using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repository.Data
{
    public class TicketHistoryRepository : GeneralRepository<MyContext, TicketHistory, int>
    {
        public TicketHistoryRepository(MyContext myContext) : base(myContext)
        {

        }
    }
}
