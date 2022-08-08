using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BlazingState.WebAssembly
{
    internal class AutoStateManager : IAutoStateManager
    {
        private const string RendererFieldName = "_renderer";
        private const string ComponentsFieldName = "_componentStateByComponent";
        private const string StateHasChangedMethodName = "StateHasChanged";

        private static FieldInfo? rendererField;
        private static Func<Renderer, object>? getComponentsFieldValue;
        private static MethodInfo? getAutoStateComponentsMethod;

        private static Dictionary<Type, MethodInfo?>? componentStateMethods;

        private static WebAssemblyHost? host;
        internal static WebAssemblyHost? Host
        {
            get => host;
            set
            {
                host = value;
                if (host is not null)
                {
                    Success = preInitSuccess;
                }
            }
        }

        private static Renderer? renderer;

        internal static bool preInitSuccess = false;
        internal static bool Success { get; private set; }

        static AutoStateManager()
        {
            rendererField = typeof(WebAssemblyHost).GetField(RendererFieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (rendererField is null)
                return;
            var componentsField = typeof(Renderer).GetField(ComponentsFieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (componentsField is null)
                return;

            getComponentsFieldValue = componentsField.CreateGetFieldDelegate<Renderer, object>();
            if (getComponentsFieldValue is null)
                return;

            getAutoStateComponentsMethod = typeof(AutoStateManager)
                .GetMethod(nameof(GetAutoStateComponents), BindingFlags.NonPublic | BindingFlags.Static)?
                .MakeGenericMethod(componentsField.FieldType.GetGenericArguments());
            if (getAutoStateComponentsMethod is null)
                return;

            componentStateMethods = Assembly.GetEntryAssembly()!.GetTypes()
                    .Where(t => typeof(ComponentBase).IsAssignableFrom(t))  // Check if type is component
                    // Check if state observer are used
                    .Where(t => t.GetRuntimeProperties().Any(x => typeof(INonGenericStateObserver).IsAssignableFrom(x.PropertyType)) ||
                        t.GetRuntimeFields().Any(x => typeof(INonGenericStateObserver).IsAssignableFrom(x.FieldType))
                    )
                    .ToDictionary(
                        t => t,
                        t => t.GetMethod(StateHasChangedMethodName, BindingFlags.NonPublic | BindingFlags.Instance, Array.Empty<Type>())
                    );

            preInitSuccess = true;
        }

        private static bool LoadRendererIfNull()
        {
            if (renderer is null)
            {
                renderer = rendererField?.GetValue(host) as Renderer;
            }

            return renderer is not null;
        }

        internal static IEnumerable<AutoComponent> GetAutoStateComponents<TKey, TValue>(IDictionary<TKey, TValue> data)
            where TKey : IComponent
        {
            foreach (var comp in data.Keys)
            {
                if (comp is not ComponentBase compBase)
                    continue;

                var compType = compBase.GetType();
                var attribute = compType.GetCustomAttribute<AutoStateAttribute>();
                if (attribute is not null)
                {
                    yield return new AutoComponent(compBase, compType, attribute.ObserverDataTypes);
                }
            }
        }

        public void NotifyAutoStateComponents(object? instance, Type dataType)
        {
            if (!Success || !LoadRendererIfNull())
                return;

            var componentStateDictionary = getComponentsFieldValue!(renderer!);
            if (componentStateDictionary is null)
                return;

            var componentStates = getComponentsFieldValue(renderer!);
            IEnumerable<AutoComponent> compStates = (IEnumerable<AutoComponent>)getAutoStateComponentsMethod!.Invoke(null, new object[] { componentStates! })!;

            foreach (var comp in compStates)
            {
                if (ReferenceEquals(comp.Instance, instance))
                {
                    continue;
                }

                if (!componentStateMethods!.TryGetValue(comp.ComponentType, out var stateHasChangedMethod))
                    continue;

                if (comp.ObserverDataTypes is not null && !comp.ObserverDataTypes.Contains(dataType))
                {
                    // Component not enabled for current data type
                    continue;
                }

                stateHasChangedMethod?.Invoke(comp.Instance, Array.Empty<object>());
            }
        }
    }
}
