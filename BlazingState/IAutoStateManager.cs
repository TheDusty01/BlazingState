using System;

namespace BlazingState
{
    internal interface IAutoStateManager
    {
        void NotifyAutoStateComponents(object? instance, Type dataType);
    }
}
