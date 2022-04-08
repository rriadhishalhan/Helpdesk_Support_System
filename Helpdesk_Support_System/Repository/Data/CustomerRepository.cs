using API.Context;
using API.Models;
using API.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repository.Data
{
    public class CustomerRepository : GeneralRepository<MyContext, Customer, string>
    {
        private readonly MyContext myContext;

        public CustomerRepository(MyContext myContext) : base(myContext)
        {
            this.myContext = myContext;
        }

        public int RegisterCustomer(RegisterCustomerVM registerCustomerVM)
        {
            var custCount = myContext.Customers.ToList().Count;
            var idFormat = "1100" + (custCount + 1).ToString();
            var result = 0;

            if (custCount != 0)
            {
                var LastId = int.Parse(myContext.Customers.OrderBy(e => e.Id)
                    .Select(e => e.Id).LastOrDefault().ToString());
                idFormat = (LastId + 1).ToString();
            }

            var emailCustomer = myContext.Customers.Where(c => c.Email == registerCustomerVM.Email).FirstOrDefault();

            if (emailCustomer != null)
            {
                result = -1;
                return result;
            }
            else
            {
                string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
                string encryptedPassword = BCrypt.Net.BCrypt.HashPassword(registerCustomerVM.Password, salt);

                Customer cust = new Customer
                {
                    Id = idFormat,
                    First_name = registerCustomerVM.first_name,
                    Last_name = registerCustomerVM.last_name,
                    Phone_number = registerCustomerVM.phone_number,
                    Email = registerCustomerVM.Email,
                    Password = encryptedPassword,
                };

                myContext.Customers.Add(cust);

                result = myContext.SaveChanges();
            }

            return result;
        }

        public int Login(LoginVM loginVM)
        {
            var customerData = myContext.Customers.Where(c => c.Email == loginVM.Email).FirstOrDefault();
            if (customerData == null)
            {
                return -1;
            }

            var isPasswordCorrect = BCrypt.Net.BCrypt.Verify(loginVM.Password, customerData.Password);
            if (!isPasswordCorrect)
            {
                return -2;
            }

            return 0;
        }

        public ICollection CustomerTickets(string customerId)
        {
            var customerTickets = (from c in myContext.Customers
                                   join t in myContext.Tickets on c.Id equals t.Customer_Id
                                   join ctgs in myContext.Categories on t.Category_Id equals ctgs.Id 
                                   where t.Customer_Id == customerId
                                   select new { 
                                        Ticket_Id = t.Id,
                                        Category = ctgs.Name,
                                        Issue = t.Issue,
                                        Solution = t.Solution,
                                        Feedback = t.Feedback
                                   }).ToList();
            return customerTickets;
        }

        public ICollection CustomerTicketHistory(string customerId, string ticketId)
        {
            var customerTicketHistory = (from c in myContext.Customers
                                         join t in myContext.Tickets on c.Id equals t.Customer_Id
                                         join th in myContext.TicketHistories on t.Id equals th.Ticket_Id
                                         join e in myContext.Employees on th.Employee_Id equals e.Id
                                         join p in myContext.Positions on e.Position_id equals p.Id
                                         where t.Customer_Id == customerId && th.Ticket_Id == ticketId
                                         orderby th.Start_date ascending
                                         select new
                                         {
                                             Date = th.Start_date.ToString("dddd, dd MMMM yyyy HH:mm:ss"),
                                             Status = th.Status.ToString(),
                                             Employee_name = $"{e.First_name} {e.Last_name} ({p.Name})"
                                         }).ToList();
            return customerTicketHistory;
        }
    }
}
