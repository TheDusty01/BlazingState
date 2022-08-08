using System;

namespace BlazingState.WebAssembly
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoStateAttribute : Attribute
    {
        public Type[]? ObserverDataTypes { get; init; }

        public AutoStateAttribute(params Type[] observerDataTypes)
        {
            ObserverDataTypes = observerDataTypes.Length == 0 ? null : observerDataTypes;
        }
    }
}
