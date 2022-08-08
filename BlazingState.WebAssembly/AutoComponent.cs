using Microsoft.AspNetCore.Components;
using System;

namespace BlazingState.WebAssembly
{
    internal struct AutoComponent
    {
        public ComponentBase Instance { get; init; }
        public Type ComponentType { get; init; }
        public Type[]? ObserverDataTypes { get; init; }

        public AutoComponent(ComponentBase component, Type componentType, Type[]? observerDataTypes)
        {
            Instance = component;
            ComponentType = componentType;
            ObserverDataTypes = observerDataTypes;
        }
    }
}
