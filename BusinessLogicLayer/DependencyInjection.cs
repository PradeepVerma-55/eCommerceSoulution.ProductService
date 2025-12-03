using BusinessLogicLayer.Mappers;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Validators;
using DataAccessLayer.Repositories;
using DataAccessLayer.RepositoryContracts;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using FluentValidation.AspNetCore;
using BusinessLogicLayer.RabbitMQ;

namespace BusinessLogicLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
        {
            // Add Business Logic Layer services here to IOC Container

            services.AddAutoMapper(typeof(ProductAddRequestToProductMappingProfile).Assembly);

            // Register Products service that implements business logic
            services.AddScoped<ServiceContracts.IProductsService, ProductsService>();

            // Register repository interface to its implementation
            services.AddScoped<IProductsRepository, ProductsRepository>();

            // Register validators if FluentValidation is used
            // This will scan the current assembly for any validators
            services.AddValidatorsFromAssembly(typeof(ProductAddRequestValidator).Assembly);
            // Register RabbitMQ Publisher
            services.AddTransient<IRabbitMQPublisher, RabbitMQPublisher>();
            return services;
        }
    }
}
