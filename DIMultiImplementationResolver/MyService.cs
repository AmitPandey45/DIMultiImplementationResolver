using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIMultiImplementationResolver
{
    public class MyService
    {
        private readonly ICustomLogger customLogger;

        public MyService(Func<ServiceType, ICustomLogger> serviceResolver)
        {
            customLogger = serviceResolver(ServiceType.FileLogger);
        }

        public MyService(ServiceResolver serviceResolver)
        {
            customLogger = serviceResolver(ServiceType.FileLogger);
        }

        public MyService(ICustomLogger customLogger)
        {
            this.customLogger = customLogger;
        }

        public void Add(int a, int b)
        {
            customLogger.Write((a + b).ToString());
        }
    }
}
