using API.Context;
using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repository.Data
{
    public class CustomerRepository : GeneralRepository<MyContext, Customer, string>
    {
        public CustomerRepository(MyContext myContext) : base(myContext)
        {

        }
    }
}
