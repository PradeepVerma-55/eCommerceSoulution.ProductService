using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public static class DependencyInjection
    {
            public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
            {
                // Add Business Logic Layer services here to IOC Container
                return services;
        }
    }
}
