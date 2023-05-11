using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIMultiImplementationResolver
{
    public class ReinstateService : IReinstateService
    {
        private readonly IMember _member;

        public ReinstateService(IMembershipOrderServiceResolver resolver)
        {
            _member = resolver.Resolve(OrderTypeEnum.Reinstate);
        }

        public void Execute(string data)
        {
            _member.ProcessOrder($"This is calling from ReinstateService to reinstate the member => {data}");
        }
    }
}
