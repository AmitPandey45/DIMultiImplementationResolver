using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIMultiImplementationResolver
{
    public class MembershipOrderServiceResolver : IMembershipOrderServiceResolver
    {
        private readonly IEnumerable<IMember> _members;

        public MembershipOrderServiceResolver(IEnumerable<IMember> members)
        {
            _members = members;
        }

        public IMember Resolve(OrderTypeEnum orderType)
        {
            switch (orderType)
            {
                case OrderTypeEnum.NewMember:
                    return _members.FirstOrDefault(m => m.GetType() == typeof(MemberOnBoarding));
                case OrderTypeEnum.Renew:
                    return _members.FirstOrDefault(m => m.GetType() == typeof(RenewMembership));
                case OrderTypeEnum.Reinstate:
                    return _members.FirstOrDefault(m => m.GetType() == typeof(ReinstateMembership));
                default:
                    return null;
            }
        }
    }
}
