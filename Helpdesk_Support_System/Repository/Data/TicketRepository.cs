using API.Context;
using API.Models;
using API.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repository.Data
{
    public class TicketRepository : GeneralRepository<MyContext, Ticket, string>
    {
        private readonly MyContext myContext;

        public TicketRepository(MyContext myContext) : base(myContext)
        {
            this.myContext = myContext;
        }

        public int CreateTicket(CreateTicketVM createTicketVM)
        {
            DateTime now = DateTime.Now;
            string customerId = createTicketVM.Customer_Id.Substring(2);
            string datetime = now.ToString("ddMMyyyHHmmss");
            string categoryId = createTicketVM.Category_Id.ToString().PadLeft(3, '0');
            string ticketId = customerId + datetime + categoryId;
            
            
            myContext.Tickets.Add(new Ticket
            {
                Id = ticketId,
                Customer_Id = createTicketVM.Customer_Id,
                Category_Id = createTicketVM.Category_Id,
                Issue = createTicketVM.Issue
            });
            myContext.SaveChanges();

            myContext.TicketHistories.Add(new TicketHistory
            {
                Ticket_Id = ticketId,
                Status = Status.Terkirim,
                Start_date = now,
                //Employee_Id = "22001"
            });
            myContext.SaveChanges();

            return 1;

        }
    }
}
