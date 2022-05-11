using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazingState
{
    public class StateObserver<T>
    {
        private readonly WeakEventHandler<object> subscriber = new WeakEventHandler<object>();
        private readonly EqualityComparer<T> equalityComparer;
        private T? currentValue;

        public T? Value
        {
            get => currentValue;
            set
            {
                if (!equalityComparer.Equals(currentValue, Value))
                    return;

                currentValue = value;
                NotifyStateChangedInternalAsync();
            }
        }

        public StateObserver(EqualityComparer<T>? equalityComparer = null)
        {
            this.equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
        }

        public StateObserver(T initialValue, EqualityComparer<T>? equalityComparer = null) : this(equalityComparer)
        {
            Value = initialValue;
        }

        private async void NotifyStateChangedInternalAsync()
        {
            await NotifyStateChangedAsync();
        }

        public Task NotifyStateChangedAsync()
        {
            return subscriber.InvokeAsync();
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
