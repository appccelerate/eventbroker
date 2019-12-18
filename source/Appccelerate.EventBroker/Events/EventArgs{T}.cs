namespace Appccelerate.EventBroker.Events
{
    using System;

    public class EventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgs{T}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public EventArgs(T value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the contained value.
        /// </summary>
        public T Value
        {
            get; private set;
        }
    }
}