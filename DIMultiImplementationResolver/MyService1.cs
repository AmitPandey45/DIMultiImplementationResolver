using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIMultiImplementationResolver
{
    internal class MyService1
    {
        private readonly ICustomLogger customLogger;

        public MyService1(Func<ServiceType, ICustomLogger> serviceResolver)
        {
            customLogger = serviceResolver(ServiceType.FileLogger);
        }

        public MyService1(ServiceResolver serviceResolver)
        {
            customLogger = serviceResolver(ServiceType.DbLogger);
        }

        public void Add(int a, int b)
        {
            customLogger.Write((a + b).ToString());
        }
    }
}
