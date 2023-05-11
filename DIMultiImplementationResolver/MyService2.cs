using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIMultiImplementationResolver
{
    internal class MyService2
    {
        private readonly ICustomLogger customLogger;

        public MyService2(Func<ServiceType, ICustomLogger> serviceResolver)
        {
            customLogger = serviceResolver(ServiceType.EventLogger);
        }

        public MyService2(ServiceResolver serviceResolver)
        {
            customLogger = serviceResolver(ServiceType.EventLogger);
        }

        public void Add(int a, int b)
        {
            customLogger.Write((a + b).ToString());
        }
    }
}
