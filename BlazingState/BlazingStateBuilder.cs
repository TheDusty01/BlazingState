using Microsoft.Extensions.DependencyInjection;

namespace BlazingState
{
    public interface IBlazingStateBuilder
    {
        public IServiceCollection Services { get; }
    }

    internal class BlazingStateBuilder : IBlazingStateBuilder
    {
        public IServiceCollection Services { get; }

        public BlazingStateBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
