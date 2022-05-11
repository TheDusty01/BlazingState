using Microsoft.Extensions.DependencyInjection;
using System;

namespace BlazingState
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStateObserver<T>(this IServiceCollection services)
        {
            services.AddScoped<StateObserver<T>>();

            return services;
        }

        public static IServiceCollection AddStateObserver<T>(this IServiceCollection services, T initialValue)
        {
            services.AddScoped(sp => new StateObserver<T>(initialValue));

            return services;
        }

        public static IServiceCollection AddStateObserver<T>(this IServiceCollection services, Func<IServiceProvider, T> implementationFactory)
        {
            services.AddScoped(sp => new StateObserver<T>(implementationFactory(sp)));

            return services;
        }
    }
}
