using API.Context;
using API.Models;
using API.Utils;
using API.ViewModel;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MimeKit.Text;
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
                    First_name = registerCustomerVM.First_Name,
                    Last_name = registerCustomerVM.Last_Name,
                    Phone_number = registerCustomerVM.Phone_Number,
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

        public int ForgotPassword(ForgotPasswordVM forgotPasswordVM)
        {
            //periksa apakah email tersebut terdapat pada table customer
            Customer customer = myContext.Customers.Where(c => c.Email == forgotPasswordVM.Email).FirstOrDefault();
            if (customer == null)
            {
                return -1; // email tidak terdaftar
            }

            //update data otp, expiredDate dan IsUsed pada customer
            customer.OTP = new Random().Next(100000, 1000000); //Generate OTP from random number
            customer.ExpiredDate = DateTime.Now.AddMinutes(5);
            customer.IsUsed = false;
            myContext.Attach(customer);
            myContext.Entry(customer).State = EntityState.Modified;
            myContext.SaveChanges();

            //persiapan mengirim email
            string receiverEmail = forgotPasswordVM.Email;
            string subject = "Forgot Password OTP";
            string body = $"Email: {customer.Email}\nOTP: {customer.OTP}\nExpired Date: {customer.ExpiredDate.ToString()}";

            //mengirim email
            Email email = new Email(receiverEmail);
            email.Create(subject, body);
            email.Send();

            return 1; //sukses
        }

        public ICollection CustomerTickets(string customerId)
        {
            var customerTickets = (from c in myContext.Customers
                                   join t in myContext.Tickets on c.Id equals t.Customer_Id
                                   join ctgs in myContext.Categories on t.Category_Id equals ctgs.Id
                                   join th in myContext.TicketHistories on t.Id equals th.Ticket_Id
                                   where t.Customer_Id == customerId && th.Start_date == myContext.TicketHistories.Where(thn => thn.Ticket_Id == t.Id).Max(thn => thn.Start_date)
                                   select new
                                   {
                                       Ticket_Id = t.Id,
                                       Category = ctgs.Name,
                                       Issue = t.Issue,
                                       Solution = t.Solution,
                                       Feedback = t.Feedback,
                                       Status = th.Status,
                                   }).ToList();
            return customerTickets;
        }

        public ICollection CustomerTicketHistory(string customerId, string ticketId)
        {
            var customerTicketHistory = (from c in myContext.Customers
                                         join t in myContext.Tickets on c.Id equals t.Customer_Id
                                         join th in myContext.TicketHistories on t.Id equals th.Ticket_Id
                                         join e in myContext.Employees on th.Employee_Id equals e.Id into temp
                                         from e in temp.DefaultIfEmpty()
                                         join post in myContext.Positions on e.Position_id equals post.Id into temp2
                                         from post in temp2.DefaultIfEmpty()
                                         where t.Customer_Id == customerId && th.Ticket_Id == ticketId
                                         orderby th.Start_date ascending
                                         select new
                                         {
                                             Date = th.Start_date.ToString("dddd, dd MMMM yyyy HH:mm:ss"),
                                             Status = th.Status.ToString(),
                                             Employee_name = e == null ? null : $"{e.First_name} {e.Last_name}",
                                             Employee_position = e == null ? null : post.Name
                                         }).ToList();
            return customerTicketHistory;
        }

        public string GetCustomerFullName(string CustomerEmail)
        {
            Customer cust = myContext.Customers.Where(c => c.Email == CustomerEmail).FirstOrDefault();

            //jika last_namenya kosong, maka hanya kembalikan first_name
            if (cust.Last_name.Equals(""))
            {
                return cust.First_name;
            }

            //jika memiliki last_name, maka concat dengan first_name
            return cust.First_name + " " + cust.Last_name;
        }

        public string GetCustomerId(string CustomerEmail)
        {
            Customer cust = myContext.Customers.Where(c => c.Email == CustomerEmail).FirstOrDefault();

           

            //jika memiliki last_name, maka concat dengan first_name
            return cust.Id;
        }

        private void SendEmail(string email, int otp, DateTime expired)
        {
            MimeMessage mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("Justice Kutch", "justice.kutch78@ethereal.email"));
            mimeMessage.To.Add(MailboxAddress.Parse(email));
            mimeMessage.Subject = "Forgot Password OTP";
            mimeMessage.Body = new TextPart(TextFormat.Plain)
            {
                Text = $"Email: {email}\nOTP: {otp}\nExpired Date: {expired.ToString()}"
            };

            SmtpClient smtp = new SmtpClient();
            smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("justice.kutch78@ethereal.email", "6X2pgyhHz6KDnx9m5J");
            smtp.Send(mimeMessage);
            smtp.Disconnect(true);
        }

        public int CountCustomer()
        {
            var dataCustomer =
            (
                from c in myContext.Customers
                select c.Id
            ).Count();

            return dataCustomer;
        }

    }
}
