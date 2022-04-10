using API.Context;
using API.Models;
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

        public int Forward(ForwardTicketVM forwardTicketVM)
        {
            try
            {
                //check apakah id ticket yang dimasukan benar dan ada pada database
                Ticket ticket = myContext.Tickets.Where(t => t.Id == forwardTicketVM.Ticket_Id).FirstOrDefault();
                if (ticket == null)
                {
                    return -1;
                }

                //check apakah id employee yang akan menerima ticket tersebut merupakan employee yang ada pada database
                Employee employeeReceiver = myContext.Employees.Where(e => e.Id == forwardTicketVM.To_Employee_Id).FirstOrDefault();
                if (employeeReceiver == null)
                {
                    return -2;
                }

                //mendapatkan informasi terkait employee yang meneruskan ticket tersebut
                Employee employeeSender = (Employee) (from t in myContext.Tickets
                                 join th in myContext.TicketHistories on t.Id equals th.Ticket_Id
                                 join e in myContext.Employees on th.Employee_Id equals e.Id into empl
                                 from e in empl.DefaultIfEmpty()
                                 where th.Start_date == myContext.TicketHistories.Where(thn => thn.Ticket_Id == t.Id).Max(thn => thn.Start_date) && th.Ticket_Id == forwardTicketVM.Ticket_Id
                                 select e).Single();

                //jika yang meneruskan ticket tidak bernilai null dan bukan merupakan admin
                if (employeeSender != null && employeeSender.Position_id != 1)
                {
                    //ambil priority id dari ticket tersebut
                    int ticketPriorityId = (int) myContext.Tickets.Where(t => t.Id == forwardTicketVM.Ticket_Id).Single().Priority_Id;

                    //kurangin workload employee dengan weight si ticket
                    employeeSender.Workload -= myContext.Priorities.Find(ticketPriorityId).Weight;
                    
                    //update data employee
                    myContext.Employees.Attach(employeeSender);
                    myContext.Entry(employeeSender).State = EntityState.Modified;
                }

                //update ticket history
                myContext.TicketHistories.Add(new TicketHistory
                {
                    Status = Status.Diteruskan,
                    Start_date = DateTime.Now,
                    Employee_Id = forwardTicketVM.To_Employee_Id,
                    Ticket_Id = forwardTicketVM.Ticket_Id
                });

                return myContext.SaveChanges();
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

                return myContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
