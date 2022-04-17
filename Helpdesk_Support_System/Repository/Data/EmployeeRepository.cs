using API.Context;
using API.Models;
using API.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repository.Data
{
    public class EmployeeRepository : GeneralRepository<MyContext, Employee, string>
    {
        private readonly MyContext myContext;
        public EmployeeRepository(MyContext myContext) : base(myContext)
        {
            this.myContext = myContext;
        }

        public ICollection MasterEmployee()
        {
            var data = myContext.Employees
                .Join(myContext.Positions, e => e.Position_id, p => p.Id, (e, p) => new { e, p })
                //.Join(myContext.TicketHistories, ep => ep.e.Id, th => th.Employee_Id, (ep, th) => new { ep, th })
                .Select(d => new
                {
                    Id = d.e.Id,
                    Position = d.p.Name,
                    FullName = $"{d.e.First_name} {d.e.Last_name}",
                    WorkLoad = d.e.Workload,
                    Email = d.e.Email,
                    PhoneNumber = d.e.Phone_number,
                    //Ticket = d.th.Ticket_Id,
                    //StatusTicket = d.th.Status.ToString(),
                });

            return data.ToList();
        }

        public int Register(RegisterEmployeeVM registerEmployeeVM)
        {
            var empCount = myContext.Employees.ToList().Count;
            var idFormat = "2200" + (empCount + 1).ToString();
            var result = 0;

            if (empCount != 0)
            {
                var LastId = int.Parse(myContext.Employees.OrderBy(e => e.Id)
                    .Select(e => e.Id).LastOrDefault().ToString());
                idFormat = (LastId + 1).ToString();
            }

            var emailEmployee = myContext.Employees.Where(e => e.Email == registerEmployeeVM.Email).FirstOrDefault();


            if (emailEmployee != null)
            {
                result = -1;
                return result;
            }
            else
            {
                string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
                string encryptedPassword = BCrypt.Net.BCrypt.HashPassword(registerEmployeeVM.Password, salt);

                Employee emp = new Employee
                {
                    Id = idFormat,
                    First_name = registerEmployeeVM.First_name,
                    Last_name = registerEmployeeVM.Last_name,
                    Email = registerEmployeeVM.Email,
                    Password = encryptedPassword,
                    Workload = 0,
                    Phone_number = registerEmployeeVM.Phone_number,
                    Position_id = registerEmployeeVM.Position_id,
                };

                myContext.Employees.Add(emp);

                result = myContext.SaveChanges();
            }
            return result;

        }

        public int Login(LoginVM loginVM)
        {
            var employeeData = myContext.Employees.Where(c => c.Email == loginVM.Email).FirstOrDefault();
            if (employeeData == null)
            {
                return -1;
            }

            var isPasswordCorrect = BCrypt.Net.BCrypt.Verify(loginVM.Password, employeeData.Password);
            if (!isPasswordCorrect)
            {
                return -2;
            }

            return 0;
        }

        public string GetEmployeeRoleName(string employeeEmail)
        {
            string roleName = (from e in myContext.Employees
                               join p in myContext.Positions on e.Position_id equals p.Id
                               where e.Email == employeeEmail
                               select p.Name).FirstOrDefault();
            return roleName;
        }

        public string GetEmployeeFullName(string employeeEmail)
        {
            Employee emp = myContext.Employees.Where(e => e.Email == employeeEmail).FirstOrDefault();

            //jika last_namenya kosong, maka hanya kembalikan first_name
            if (emp.Last_name.Equals(""))
            {
                return emp.First_name;
            }

            //jika memiliki last_name, maka concat dengan first_name
            return emp.First_name + " " + emp.Last_name;
        }

        public string GetEmployeeId(string EmployeeEmail)
        {
            Employee emp = myContext.Employees.Where(c => c.Email == EmployeeEmail).FirstOrDefault();



            return emp.Id;
        }

        public ICollection EmployeeTickets(string employeeId)
        {
            var employeeTickets = (from c in myContext.Customers
                                   join t in myContext.Tickets on c.Id equals t.Customer_Id
                                   join th in myContext.TicketHistories on t.Id equals th.Ticket_Id
                                   join emp in myContext.Employees on th.Employee_Id equals emp.Id into temp1
                                   from emp in temp1.DefaultIfEmpty()
                                   join ctgs in myContext.Categories on t.Category_Id equals ctgs.Id
                                   join prio in myContext.Priorities on t.Priority_Id equals prio.Id into temp2
                                   from prio in temp2.DefaultIfEmpty()
                                   where th.Start_date == (myContext.TicketHistories.Where(thn => thn.Ticket_Id == t.Id).Max(thn => thn.Start_date)) && th.Employee_Id == employeeId
                                   select new
                                   {
                                       Ticket_Id = t.Id,
                                       Employee_Id = th.Employee_Id,
                                       Employee_Name = (emp == null) ? null : $"{emp.First_name} {emp.Last_name}",
                                       Priority = (prio == null) ? null : prio.Name,
                                       Priority_Weight = (prio == null) ? null : $"{prio.Weight}",
                                       Category = ctgs.Name,
                                       Issue = t.Issue,
                                       Solution = t.Solution,
                                       Feedback = t.Feedback,
                                       Status = th.Status,
                                   }).ToList();
            return employeeTickets;
        }
    }
}
