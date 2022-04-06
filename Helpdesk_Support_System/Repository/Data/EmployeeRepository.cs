using API.Context;
using API.Models;
using API.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System;
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

        //public int Register(RegisterEmployeeVM registerEmployeeVM)
        //{
        //    if (myContext.Employees.SingleOrDefault(e => e.Email == registerEmployeeVM.Email) != null)
        //    {
        //        return -1; //email already used
        //    }

        //    if (myContext.Employees.SingleOrDefault(e => e.Phone_number == registerEmployeeVM.Phone_number) != null)
        //    {
        //        return -2; //phone number already used
        //    }

        //    Employee newEmployee = new Employee
        //    {
        //        Email = registerEmployeeVM.Email,
        //        First_name = registerEmployeeVM.First_name,
        //        Last_name = registerEmployeeVM.Last_name,
        //        Workload = registerEmployeeVM.Workload,
        //        Phone_number = registerEmployeeVM.Phone_number,
        //        Position_id = registerEmployeeVM.Position_id
        //    };
        //    myContext.Employees.Add(newEmployee);

        //    //myContext.SaveChanges();

        //    Account newAccount = new Account
        //    {
        //        Email = newEmployee.Email,
        //        Password = registerEmployeeVM.Password,
        //        Role_id = 2
        //    };
        //    myContext.Accounts.Add(newAccount);
        //    myContext.SaveChanges();

        //    return 1;
        //}

    }
}
