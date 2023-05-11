using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIMultiImplementationResolver
{
    public class RenewService : IRenewService
    {
        private readonly IMember _member;

        public RenewService(IMembershipOrderServiceResolver resolver)
        {
            _member = resolver.Resolve(OrderTypeEnum.Renew);
        }

        public void Execute(string data)
        {
            _member.ProcessOrder($"This is calling from RenewService to renew the member => {data}");
        }
    }
}
