using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BlazingState
{
    /// <summary>
    /// Use <see cref="IStateObserver{T}"/> instead.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StateObserver<T> : IStateObserver<T>
    {
        private readonly WeakEventHandler<object> subscriber = new WeakEventHandler<object>();
        private readonly EqualityComparer<T> equalityComparer;
        private readonly IAutoStateManager? autoStateManager;

        private T? currentValue;

        public T? Value
        {
            get => currentValue;
            set
            {
                if (SetValueInternal(value))
                    NotifyStateChangedInternalAsync();
            }
        }

        #region Init
        public StateObserver(EqualityComparer<T>? equalityComparer = null)
        {
            this.equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
        }

        internal StateObserver(IAutoStateManager? autoStateManager, EqualityComparer<T>? equalityComparer = null) : this(equalityComparer)
        {
            this.autoStateManager = autoStateManager;
        }

        public StateObserver(T initialValue, EqualityComparer<T>? equalityComparer = null) : this(equalityComparer)
        {
            currentValue = initialValue;
        }

        internal StateObserver(IAutoStateManager? autoStateManager, T initialValue, EqualityComparer<T>? equalityComparer = null) : this(initialValue, equalityComparer)
        {
            this.autoStateManager = autoStateManager;
        }
        #endregion

        private async void NotifyStateChangedInternalAsync(object? instance = null)
        {
            await NotifyStateChangedAsync(instance);
        }

        private bool SetValueInternal(T? newValue)
        {
            if (!equalityComparer.Equals(currentValue, Value))
                return false;

            currentValue = newValue;
            return true;
        }

        public void SetValue(T? newValue, object? instance = null)
        {
            if (SetValueInternal(newValue))
                NotifyStateChangedInternalAsync(instance);
        }

        public Task SetValueAsync(T? newValue, object? instance = null)
        {
            if (SetValueInternal(newValue))
                return NotifyStateChangedAsync(instance);

            return Task.CompletedTask;
        }

        public Task NotifyStateChangedAsync(object? instance = null)
        {
            autoStateManager?.NotifyAutoStateComponents(instance, typeof(T));
            return subscriber.InvokeAsync(instance);
        }

        public void Register(object instance, Action callback)
        {
            subscriber.Register(instance, () =>
            {
                callback();
                return Task.CompletedTask;
            });
        }

        public void Register(object instance, Func<Task> callback)
        {
            subscriber.Register(instance, callback);
        }

        public bool Unregister(object instance)
        {
            return subscriber.Unregister(instance);
        }

        public int GetSubscriberCount()
        {
            return subscriber.Callbacks.Count();
        }
    }
}
