using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public static class DependencyInjection
    {
        // Extension methods for setting up data access layer dependencies would go here
        public static IServiceCollection AddDataAccessLayer(this IServiceCollection services)
        {
            // Example: services.AddDbContext<MyDbContext>(options => ...);
            // Add Data Access Layer services here to IOC Container

            return services;
        }
    }
}
