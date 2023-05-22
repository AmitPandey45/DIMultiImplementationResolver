using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIMultiImplementationResolver
{
    public static class Startup
    {
        public static IServiceCollection ConfigureService()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddTransient<MyService>();
            services.AddTransient<MyService1>();
            services.AddTransient<MyService2>();

            services.AddTransient<IAwsS3Client, AwsS3Client>();

            services.AddTransient<ICustomLogger, FileLogger>();
            services.AddTransient<ICustomLogger, DbLogger>();
            services.AddTransient<ICustomLogger, EventLogger>();
            services.AddTransient<FileLogger>();
            services.AddTransient<DbLogger>();
            services.AddTransient<EventLogger>();

            services.AddTransient<ServiceResolver>(serviceProvider => serviceTypeName =>
            {
                switch (serviceTypeName)
                {
                    case ServiceType.FileLogger:
                        return serviceProvider.GetService<FileLogger>();
                    case ServiceType.DbLogger:
                        return serviceProvider.GetService<DbLogger>();
                    case ServiceType.EventLogger:
                        return serviceProvider.GetService<EventLogger>();
                    default:
                        return null;
                }
            });

            services.AddTransient<IMember, MemberOnBoarding>();
            services.AddTransient<IMember, RenewMembership>();
            services.AddTransient<IMember, ReinstateMembership>();
            services.AddTransient<MembershipOrderServiceResolver>(serviceProvider => new MembershipOrderServiceResolver(serviceProvider.GetServices<IMember>()
            ));


            services.AddTransient<IMembershipOrderServiceResolver, MembershipOrderServiceResolver>();

            services.AddTransient<IOnboardService, OnboardService>();
            services.AddTransient<IRenewService, RenewService>();
            services.AddTransient<IReinstateService, ReinstateService>();

            ////services.BuildServiceProvider();

            return services;
        }
    }
}
