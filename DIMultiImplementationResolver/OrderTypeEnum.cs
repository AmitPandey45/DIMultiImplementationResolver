using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIMultiImplementationResolver
{
    public enum OrderTypeEnum
    {
        [Description("Newmember")]
        NewMember = 0,

        [Description("Renew")]
        Renew = 1,

        [Description("Reinstate")]
        Reinstate = 2,
    }
}
