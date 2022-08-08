using System;
using System.Threading.Tasks;

namespace BlazingState
{
    /// <summary>
    /// This type is used for framework internals. Use <see cref="IStateObserver{T}"/> instead.
    /// </summary>
    public interface INonGenericStateObserver { }

    public interface IStateObserver<T> : INonGenericStateObserver
    {
        /// <summary>
        /// Sets/gets the value of this observer.
        /// Doesn't wait for all components to finish rerendering.
        /// </summary>
        public T? Value { get; set; }

        /// <summary>
        /// Sets the value of this observer with optionally specifying from which instance the change came.
        /// Doesn't wait for all components to finish rerendering.
        /// </summary>
        /// <param name="newValue"></param>
        /// <param name="instance"></param>
        public void SetValue(T? newValue, object? instance = null);

        /// <summary>
        /// Sets the value of this observer with optionally specifying from which instance the change came.
        /// Waits for all components to finish rerendering.
        /// </summary>
        /// <param name="newValue"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public Task SetValueAsync(T? newValue, object? instance = null);

        /// <summary>
        /// Notifies all components that the state changed.
        /// Waits for all components to finish rerendering.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public Task NotifyStateChangedAsync(object? instance = null);

        public void Register(object instance, Action callback);
        public void Register(object instance, Func<Task> callback);
        public bool Unregister(object instance);
        public int GetSubscriberCount();
    }
}
