using API.Context;
using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repository.Data
{
    public class PriorityRepository : GeneralRepository<MyContext, Priority, int>
    {
        public PriorityRepository(MyContext myContext) : base(myContext)
        {

        }
    }
}
