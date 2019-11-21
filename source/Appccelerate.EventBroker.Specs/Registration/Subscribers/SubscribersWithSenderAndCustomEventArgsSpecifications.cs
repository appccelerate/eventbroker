//-------------------------------------------------------------------------------
// <copyright file="SubscribersWithSenderAndCustomEventArgsSpecifications.cs" company="Appccelerate">
//   Copyright (c) 2008-2015
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------

namespace Appccelerate.EventBroker.Registration.Subscribers
{
    using System;
    using System.Collections.Generic;
    using Appccelerate.EventBroker.Handlers;
    using Appccelerate.Events;
    using FluentAssertions;
    using Machine.Specifications.Annotations;
    using Xbehave;

    public class SubscribersWithSenderAndCustomEventArgsSpecifications
    {
        private readonly EventArgs<string> eventArgs = new EventArgs<string>("test");

        private EventBroker eventBroker;
        private CustomEvent.EventPublisher publisher;

        [Background]
        public void SetupEventBroker()
        {
            "Establish an event broker".x(() =>
                this.eventBroker = new EventBroker());

            "Establish an registered publisher".x(() =>
            {
                this.publisher = new CustomEvent.EventPublisher();
                this.eventBroker.Register(this.publisher);
            });
        }

        [Scenario]
        public void RegisterUnregister(
            SubscriberWithSenderAndCustomEventArgs subscriber)
        {
            "Establish a subscriber".x(() =>
                subscriber = new SubscriberWithSenderAndCustomEventArgs());

            "When registering, firing an event, unregistering and firing another event".x(() =>
            {
                this.eventBroker.Register(subscriber);
                this.publisher.FireEvent(this.eventArgs);
                this.eventBroker.Unregister(subscriber);
                this.publisher.FireEvent(this.eventArgs);
            });

            "It should call the handler method on the subscriber with generic event arguments from the publisher".x(() =>
                subscriber.ReceivedEventArgValues.Should().Contain(this.eventArgs));

            "It should call the handler method only as long as the subscriber is registered".x(() =>
                subscriber.ReceivedEventArgValues.Should().HaveCount(1, "event should not be routed anymore after subscriber is unregistered."));
        }

        [Scenario]
        public void RegisterUnregisterWithoutAttribute(
            SubscriberWithoutRegistrationAttributeButWithSenderAndCustomEventArgs subscriber)
        {
            "Establish a subscriber".x(() =>
                subscriber = new SubscriberWithoutRegistrationAttributeButWithSenderAndCustomEventArgs());

            "When registering, firing an event, unregistering and firing another event".x(() =>
            {
                this.eventBroker.SpecialCasesRegistrar.AddSubscription<EventArgs<string>>(CustomEvent.EventTopic, subscriber, subscriber.Handle, new OnPublisher());
                this.publisher.FireEvent(this.eventArgs);
                this.eventBroker.SpecialCasesRegistrar.RemoveSubscription<EventArgs<string>>(CustomEvent.EventTopic, subscriber, subscriber.Handle);
                this.publisher.FireEvent(this.eventArgs);
            });

            "It should call the handler method on the subscriber with generic event arguments from the publisher".x(() =>
                subscriber.ReceivedEventArgValues.Should().Contain(this.eventArgs));

            "It should call the handler method only as long as the subscriber is registered".x(() =>
                subscriber.ReceivedEventArgValues.Should().HaveCount(1, "event should not be routed anymore after subscriber is unregistered."));
        }

        public class SubscriberWithSenderAndCustomEventArgs : SubscriberWithSenderAndCustomEventArgsBase
        {
            [EventSubscription(CustomEvent.EventTopic, typeof(OnPublisher)), UsedImplicitly]
            public void Handle(object sender, EventArgs<string> eventArgs)
            {
                this.ReceivedEventArgValues.Add(eventArgs);
            }
        }

        public class SubscriberWithoutRegistrationAttributeButWithSenderAndCustomEventArgs : SubscriberWithSenderAndCustomEventArgsBase
        {
            public void Handle(object sender, EventArgs<string> eventArgs)
            {
                this.ReceivedEventArgValues.Add(eventArgs);
            }
        }

        public class SubscriberWithSenderAndCustomEventArgsBase
        {
            protected SubscriberWithSenderAndCustomEventArgsBase()
            {
                this.ReceivedEventArgValues = new List<EventArgs>();
            }

            public List<EventArgs> ReceivedEventArgValues { get; private set; }
        }
    }
}