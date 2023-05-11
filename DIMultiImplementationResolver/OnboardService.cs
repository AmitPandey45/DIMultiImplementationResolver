using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIMultiImplementationResolver
{
    public class OnboardService : IOnboardService
    {
        private readonly IMember _member;

        public OnboardService(IMembershipOrderServiceResolver resolver)
        {
            _member = resolver.Resolve(OrderTypeEnum.NewMember);
        }

        public void Execute(string data)
        {
            _member.ProcessOrder($"This is calling from OnboardService to onboard the member => {data}");
        }
    }
}
