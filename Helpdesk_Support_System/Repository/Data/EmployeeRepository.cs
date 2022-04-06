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
                Employee emp = new Employee
                {
                    Id = idFormat,
                    First_name = registerEmployeeVM.First_name,
                    Last_name = registerEmployeeVM.Last_name,
                    Email = registerEmployeeVM.Email,
                    Password = registerEmployeeVM.Password,
                    Workload = 0,
                    Phone_number = registerEmployeeVM.Phone_number,
                    Position_id = registerEmployeeVM.Position_id,
                };

                myContext.Employees.Add(emp);

                result = myContext.SaveChanges();
            }
            return result;

        }

    }
}
