namespace Appccelerate.EventBroker.Registration.Subscribers
{
    using System;
    using Appccelerate.EventBroker.Internals;

    public class Publisher
    {
        [EventPublication(SimpleEvent.EventTopic)]
        public event EventHandler<EventArgs<string>> Event;

        public void FireEvent(string value)
        {
            this.Event?.Invoke(this, new EventArgs<string>(value));
        }
    }
}