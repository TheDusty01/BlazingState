using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BlazingState
{
    internal class WeakEventHandler<TKey>
        where TKey : class
    {
        public ConditionalWeakTable<TKey, Func<Task>> Callbacks { get; private set; } = new ConditionalWeakTable<TKey, Func<Task>>();

        public void Register(TKey instance, Func<Task> callback)
        {
            Callbacks.AddOrUpdate(instance, callback);
        }

        public bool Unregister(TKey instance)
        {
            return Callbacks.Remove(instance);
        }

        public async Task InvokeAsync(object? calledByInstance = null)
        {
            HashSet<Task> tasks = new HashSet<Task>();
            foreach ((var instance, var callback) in Callbacks)
            {
                // Do not notify calling instance again
                if (ReferenceEquals(instance, calledByInstance))
                    continue;

                tasks.Add(callback.Invoke());
            }

            await Task.WhenAll(tasks);
        }
    }
}
