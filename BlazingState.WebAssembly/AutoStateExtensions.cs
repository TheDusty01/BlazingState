using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace BlazingState.WebAssembly
{
    public static class AutoStateExtensions
    {
        public static IBlazingStateBuilder AddAutoState(this IBlazingStateBuilder builder)
        {
            builder.Services.TryAddSingleton<IAutoStateManager, AutoStateManager>();

            return builder;
        }

        public static WebAssemblyHost UseAutoState(this WebAssemblyHost host)
        {
            if (host.Services.GetService<IAutoStateManager>() is not AutoStateManager)
                throw new InvalidOperationException($"Cannot retrieve AutoStateManager. Did you call {nameof(AddAutoState)} during startup?");

            AutoStateManager.Host = host;

            return host;
        }
    }
}
