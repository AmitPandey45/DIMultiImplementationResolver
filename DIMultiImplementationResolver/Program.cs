
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System;

namespace DIMultiImplementationResolver
{
    public delegate ICustomLogger ServiceResolver(ServiceType serviceType);

    public class Program
    {
        public static Func<ServiceType, ICustomLogger> GetServiceResolver()
        {
            ICustomLogger fileLogger = new FileLogger();
            ICustomLogger dbLogger = new DbLogger();
            ICustomLogger eventLogger = new EventLogger();

            return serviceType =>
            {
                switch (serviceType)
                {
                    case ServiceType.FileLogger:
                        return fileLogger;
                    case ServiceType.DbLogger:
                        return dbLogger;
                    case ServiceType.EventLogger:
                        return eventLogger;
                    default:
                        throw new ArgumentException($"Invalid service type: {serviceType}");
                }
            };
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            IServiceCollection services = Startup.ConfigureService();
            var serviceProvider = services.BuildServiceProvider();

            var serviceResolver = serviceProvider.GetService<ServiceResolver>();
            var myService = new MyService(serviceResolver(ServiceType.EventLogger));
            var myService00 = new MyService(serviceResolver);
            ////var myService00 = serviceProvider.GetService<MyService>();
            ///var fsdf = new MyService(ServiceResolver(ServiceType.FileLogger));
            myService.Add(1, 2);

            var funcWithServiceType = GetServiceResolver();

            var fileLogger = funcWithServiceType(ServiceType.FileLogger);
            var dbLogger = funcWithServiceType(ServiceType.DbLogger);
            var eventLogger = funcWithServiceType(ServiceType.EventLogger);

            //var myService424 = new MyService(serviceType => {
            //    switch (serviceType)
            //    {
            //        case ServiceType.FileLogger:
            //            return new FileLogger();
            //        case ServiceType.DbLogger:
            //            return new DbLogger();
            //        case ServiceType.EventLogger:
            //            return new EventLogger();
            //        default:
            //            throw new ArgumentException($"Unknown service type: {serviceType}");
            //    }
            //});


            var myService1 = new MyService1(serviceResolver);
            ////var myService = serviceProvider.GetService<MyService>();
            myService1.Add(1, 2);

            var myService2 = new MyService2(serviceResolver);
            ////var myService = serviceProvider.GetService<MyService>();
            myService2.Add(1, 2);
        }
    }
}