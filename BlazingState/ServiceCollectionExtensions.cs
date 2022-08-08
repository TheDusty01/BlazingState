using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace BlazingState
{
    public static class ServiceCollectionExtensions
    {
        public static IBlazingStateBuilder AddBlazingState(this IServiceCollection services)
        {
            return new BlazingStateBuilder(services);
        }

        public static IBlazingStateBuilder AddStateObserver<T>(this IBlazingStateBuilder builder)
        {
            builder.Services.AddScoped<IStateObserver<T>, StateObserver<T>>(sp => new StateObserver<T>(sp.GetService<IAutoStateManager>()));

            return builder;
        }

        public static IBlazingStateBuilder AddStateObserver<T>(this IBlazingStateBuilder builder, T initialValue)
        {
            builder.Services.AddScoped<IStateObserver<T>, StateObserver<T>>(sp => new StateObserver<T>(sp.GetService<IAutoStateManager>(), initialValue));

            return builder;
        }

        public static IBlazingStateBuilder AddStateObserver<T>(this IBlazingStateBuilder builder, Func<IServiceProvider, T> implementationFactory)
        {
            builder.Services.AddScoped<IStateObserver<T>, StateObserver<T>> (sp => new StateObserver<T>(sp.GetService<IAutoStateManager>(), implementationFactory(sp)));

            return builder;
        }
    }
}
