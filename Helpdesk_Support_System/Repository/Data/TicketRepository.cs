using API.Context;
using API.Models;
using API.Utils;
using API.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
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
            Customer customer = myContext.Customers.Where(c => c.Id == createTicketVM.Customer_Id).FirstOrDefault();
            if (customer == null)
            {
                return -1;
            }

            Category category = myContext.Categories.Where(cat => cat.Id == createTicketVM.Category_Id).FirstOrDefault();
            if (category == null)
            {
                return -2;
            }

            if (createTicketVM.Issue == null || createTicketVM.Issue == "")
            {
                return -3;
            }

            //membuat UID untuk ticket
            DateTime now = DateTime.Now;
            string customerId = createTicketVM.Customer_Id.Substring(2);
            string datetime = now.ToString("ddMMyyyHHmmss");
            string categoryId = createTicketVM.Category_Id.ToString().PadLeft(3, '0');
            string ticketId = customerId + datetime + categoryId;
            
            //menyimpan data ticket pada table ticket
            myContext.Tickets.Add(new Ticket
            {
                Id = ticketId,
                Customer_Id = createTicketVM.Customer_Id,
                Category_Id = createTicketVM.Category_Id,
                Issue = createTicketVM.Issue
            });
            myContext.SaveChanges();

            //mencatatnya pada table ticketHistories dengan status "Terkirim"
            myContext.TicketHistories.Add(new TicketHistory
            {
                Ticket_Id = ticketId,
                Status = Status.Terkirim,
                Start_date = now
            });
            myContext.SaveChanges();


            //cari position_id dari admin
            Position positionAdmin = myContext.Positions.Where(p => p.Name == "Admin").FirstOrDefault();
            //cari data admin pada table employee
            Employee employeeAdmin = myContext.Employees.Where(e => e.Position_id == positionAdmin.Id).FirstOrDefault();

            //teruskan ticket ke admin
            //catat pada table ticketHistories dengan status "Diteruskan"
            myContext.TicketHistories.Add(new TicketHistory
            {
                Status = Status.Diteruskan,
                Start_date = DateTime.Now,
                Employee_Id = employeeAdmin.Id,
                Ticket_Id = ticketId
            });

            myContext.SaveChanges();
            return 1;

        }
        public ICollection MasterTicket()
        {
            var data = myContext.Tickets
            .Join(myContext.Categories, t => t.Category_Id, c => c.Id, (t, c) => new { t, c })
            .Join(myContext.Priorities, tc => tc.t.Priority_Id, p => p.Id, (tc, p) => new { tc, p })
            .Select(d => new
            {
                id = d.tc.t.Id,
                priority = d.p.Name,
                category = d.tc.c.Name,
                issue = d.tc.t.Issue,
                solution = d.tc.t.Solution,
                feedback = d.tc.t.Feedback,

            });

            return data.ToList();
        }

        public ICollection MasterTicket(string status)
        {
            Status statusParse;
            switch (status.ToLower())
            {
                case "terkirim":
                    statusParse = Status.Terkirim;
                    break;
                case "diteruskan":
                    statusParse = Status.Diteruskan;
                    break;
                case "dibuka":
                    statusParse = Status.Dibuka;
                    break;
                case "terjawab":
                    statusParse = Status.Terjawab;
                    break;
                case "ditutup":
                    statusParse = Status.Ditutup;
                    break;
                default:
                    throw new Exception();
            }

            var data = (from t in myContext.Tickets
                        join th in myContext.TicketHistories on t.Id equals th.Ticket_Id
                        join c in myContext.Categories on t.Category_Id equals c.Id
                        join p in myContext.Priorities on t.Priority_Id equals p.Id into prio
                        from p in prio.DefaultIfEmpty()
                        join e in myContext.Employees on th.Employee_Id equals e.Id into empl
                        from e in empl.DefaultIfEmpty()
                        where th.Start_date == myContext.TicketHistories.Where(thn => thn.Ticket_Id == t.Id).Max(thn => thn.Start_date) && th.Status == statusParse
                        select new { 
                            Id = t.Id,
                            Priority = p.Name,
                            Category = c.Name,
                            Issue = t.Issue,
                            Solution = t.Solution,
                            Feedback = t.Feedback,
                            Status = th.Status
                        }).ToList();
            return data;
        }

        public int SetPriority(SetPriorityVM setPriorityVM)
        {
            try
            {
                //check apakah id ticket yang dimasukan benar dan ada pada database
                Ticket ticket = myContext.Tickets.Where(t => t.Id == setPriorityVM.Ticket_Id).FirstOrDefault();
                if (ticket == null)
                {
                    return -1;
                }

                //check apakah id priority yang dimasukan adalah benar
                Priority priority = myContext.Priorities.Where(p => p.Id == setPriorityVM.Priority_Id).FirstOrDefault();
                if (priority == null)
                {
                    return -2;
                }

                //update ticket priority
                ticket.Priority_Id = priority.Id;
                myContext.Tickets.Attach(ticket);
                myContext.Entry(ticket).State = EntityState.Modified;

                return myContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ForwardMessageVM Forward(ForwardTicketVM forwardTicketVM)
        {
            try
            {
                //mendefinisikan beban yang dapat ditanggung employee
                const int MAX_WORKLOAD = 10;

                //check apakah id ticket yang diberikan valid
                Ticket ticket = myContext.Tickets.Where(t => t.Id == forwardTicketVM.Ticket_Id).FirstOrDefault();
                if (ticket == null)
                {
                    return new ForwardMessageVM {
                        Status_Code = 404,
                        Error_Message = "Ticket Not Found"
                    };
                }

                //mendapatkan informasi terkait employee yang meneruskan ticket tersebut
                Employee employeeSender = (Employee)(from t in myContext.Tickets
                                                     join th in myContext.TicketHistories on t.Id equals th.Ticket_Id
                                                     join e in myContext.Employees on th.Employee_Id equals e.Id
                                                     where th.Start_date == myContext.TicketHistories.Where(thn => thn.Ticket_Id == t.Id).Max(thn => thn.Start_date) && th.Ticket_Id == forwardTicketVM.Ticket_Id
                                                     select e).Single();

                //mendapatkan level saat ini
                int currentLevel = employeeSender.Position_id;

                //mendapatkan bobot dari ticket
                int ticketWeight = myContext.Priorities.Where(prio => prio.Id == ticket.Priority_Id).FirstOrDefault().Weight;

                //cari employee yang bukan admin dan bebannya tidak melebihi batas
                Employee employeeReciever = myContext.Employees
                                            .Where(e => e.Position_id == currentLevel + 1 && e.Workload + ticketWeight <= MAX_WORKLOAD)
                                            .FirstOrDefault();

                //jika tidak ada employee yang sedang tidak sibuk
                if (employeeReciever == null)
                {
                    return new ForwardMessageVM
                    {
                        Status_Code = 404,
                        Error_Message = "No Vacant Employee",
                    };
                }

                //jika yang meneruskan ticket tidak bernilai null dan bukan merupakan admin
                if (employeeSender.Position_id != 1)
                {
                    //kurangin workload employee dengan weight si ticket
                    employeeSender.Workload -= ticketWeight;
                    
                    //update data employee
                    myContext.Employees.Attach(employeeSender);
                    myContext.Entry(employeeSender).State = EntityState.Modified;
                    myContext.SaveChanges();
                }

                //update ticket history
                myContext.TicketHistories.Add(new TicketHistory
                {
                    Status = Status.Diteruskan,
                    Start_date = DateTime.Now,
                    Employee_Id = employeeReciever.Id,
                    Ticket_Id = forwardTicketVM.Ticket_Id
                });

                myContext.SaveChanges();

                return new ForwardMessageVM
                {
                    Employee_Id = employeeReciever.Id,
                    Employee_Name = $"{employeeReciever.First_name} {employeeReciever.Last_name}",
                    Employee_Position = myContext.Positions.Where(p => p.Id == employeeReciever.Position_id).Single().Name,
                    Status_Code = 200,
                };
            } catch (Exception e)
            {
                throw e;
            }
        }

        public int Open(OpenTicketVM openTicketVM)
        {
            try
            {
                //check apakah id ticket yang dimasukan benar dan ada pada database
                Ticket ticket = myContext.Tickets.Where(t => t.Id == openTicketVM.Ticket_Id).FirstOrDefault();
                if (ticket == null)
                {
                    return -1;
                }

                //check apakah id employee yang akan membuka ticket tersebut merupakan employee yang ada pada database
                Employee employeeOpener = myContext.Employees.Where(e => e.Id == openTicketVM.Opener_Employee_Id).FirstOrDefault();
                if (employeeOpener == null)
                {
                    return -2;
                }

                //jika yang meneruskan ticket tidak bernilai null dan bukan merupakan admin
                if (employeeOpener.Position_id != 1)
                {
                    //ambil priority id dari ticket tersebut
                    int ticketPriorityId = (int)myContext.Tickets.Where(t => t.Id == openTicketVM.Ticket_Id).Single().Priority_Id;

                    //kurangin workload employee dengan weight si ticket
                    employeeOpener.Workload += myContext.Priorities.Find(ticketPriorityId).Weight;

                    //update data employee
                    myContext.Employees.Attach(employeeOpener);
                    myContext.Entry(employeeOpener).State = EntityState.Modified;
                }

                //update ticket history
                myContext.TicketHistories.Add(new TicketHistory
                {
                    Status = Status.Dibuka,
                    Start_date = DateTime.Now,
                    Employee_Id = openTicketVM.Opener_Employee_Id,
                    Ticket_Id = openTicketVM.Ticket_Id
                });

                return myContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int Respond(RespondTicketVM respondTicketVM)
        {
            try
            {
                //check apakah id ticket yang dimasukan benar dan ada pada database
                Ticket ticket = myContext.Tickets.Where(t => t.Id == respondTicketVM.Ticket_Id).FirstOrDefault();
                if (ticket == null)
                {
                    return -1;
                }

                //check apakah id employee yang akan membalas ticket tersebut merupakan employee yang ada pada database
                Employee employeeResponder = myContext.Employees.Where(e => e.Id == respondTicketVM.Employee_Id).FirstOrDefault();
                if (employeeResponder == null)
                {
                    return -2;
                }

                //check apakah solusi yang diberikan tidak kosong
                if (respondTicketVM.Solution == null || respondTicketVM.Solution == "")
                {
                    return -3;
                }

                //jika yang meneruskan ticket tidak bernilai null dan bukan merupakan admin
                if (employeeResponder.Position_id != 1)
                {
                    //ambil priority id dari ticket tersebut
                    int ticketPriorityId = (int)myContext.Tickets.Where(t => t.Id == respondTicketVM.Ticket_Id).Single().Priority_Id;

                    //kurangin workload employee dengan weight si ticket
                    employeeResponder.Workload -= myContext.Priorities.Find(ticketPriorityId).Weight;

                    //update data employee
                    myContext.Employees.Attach(employeeResponder);
                    myContext.Entry(employeeResponder).State = EntityState.Modified;
                }

                //menuliskan solusi pada table ticket
                ticket.Solution = respondTicketVM.Solution;
                myContext.Tickets.Attach(ticket);
                myContext.Entry(ticket).State = EntityState.Modified;

                //update ticket history
                myContext.TicketHistories.Add(new TicketHistory
                {
                    Status = Status.Terjawab,
                    Start_date = DateTime.Now,
                    Employee_Id = respondTicketVM.Employee_Id,
                    Ticket_Id = respondTicketVM.Ticket_Id
                });

                //mengambil email dari customer
                string customerId = myContext.Tickets.Where(t => t.Id == respondTicketVM.Ticket_Id).Single().Customer_Id;
                string customerEmail = myContext.Customers.Where(c => c.Id == customerId).Single().Email;

                //persiapan mengirim email
                string receiverEmail = customerEmail;
                string subject = $"Ticket Keluhan No. {ticket.Id} Telah Terjawab";
                string body = $"Keluhan pada Ticket No. {ticket.Id} telah dijawab oleh {employeeResponder.First_name} {employeeResponder.Last_name}.\n" +
                    $"Keluhan dijawab pada tanggal {DateTime.Now.ToString()}. Berikut Solusi yang berikan:\n" +
                    $"{respondTicketVM.Solution}\n\n" +
                    $"Silahkan kirimkan Feedback anda pada sistem untuk menutup Ticket ini.\nTicket akan ditutup oleh kami dalam waktu 2 hari.";

                //mengirim email
                Email email = new Email(receiverEmail);
                email.Create(subject, body);
                email.Send();

                return myContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int CustomerFeedback(CustomerFeedbackVM customerFeedbackVM)
        {
            //check apakah id ticket yang dimasukan benar dan ada pada database
            Ticket ticket = myContext.Tickets.Where(t => t.Id == customerFeedbackVM.Ticket_Id).FirstOrDefault();
            if (ticket == null)
            {
                return -1;
            }

            //check apakah feedback yang diberikan kosong atau tidak
            if (customerFeedbackVM.Feedback == null || customerFeedbackVM.Feedback == "")
            {
                return -2;
            }

            //mengambil data employee yang memberikan solusi pada ticket tersebut
            Employee employee = (Employee)(from t in myContext.Tickets
                                                 join th in myContext.TicketHistories on t.Id equals th.Ticket_Id
                                                 join e in myContext.Employees on th.Employee_Id equals e.Id
                                                 where th.Status == Status.Terjawab && th.Ticket_Id == customerFeedbackVM.Ticket_Id
                                                 select e).First();

            //mengupdate kolom feedback pada table tb_m_tickets
            ticket.Feedback = customerFeedbackVM.Feedback;
            myContext.Tickets.Attach(ticket);
            myContext.Entry(ticket).State = EntityState.Modified;

            //membuat riwayat ticket yang berstatus tertutup
            myContext.TicketHistories.Add(new TicketHistory
            {
                Status = Status.Ditutup,
                Start_date = DateTime.Now,
                Employee_Id = employee.Id,
                Ticket_Id = customerFeedbackVM.Ticket_Id
            });

            return myContext.SaveChanges();
        }

        public int CountTicket()
        {
            var dataTicket =
            (
                from t in myContext.Tickets 
                select t.Id
            ).Count();

            return dataTicket;
        }

        public int CountProcessTicket()
        {
            var dataProcessTicket =
            (
                from t in myContext.Tickets
                where t.Feedback == null
                select t.Id
            ).Count();

            return dataProcessTicket;
        }

        public int CountClosedTicket()
        {
            var dataClosedTicket =
            (
                from t in myContext.Tickets
                where t.Feedback != null
                select t.Id
            ).Count();

            return dataClosedTicket;
        }

    }
}
