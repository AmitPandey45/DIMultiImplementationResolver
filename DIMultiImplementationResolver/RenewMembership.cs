using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIMultiImplementationResolver
{
    public class RenewMembership : IMember
    {
        public bool ProcessOrder(string data)
        {
            Console.WriteLine($"MemberOnBoarding => {data}");
            return true;
        }
    }
}
