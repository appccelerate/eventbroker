//-------------------------------------------------------------------------------
// <copyright file="SubscribersWithoutSenderAndUnwrappedEventArgsSpecifications.cs" company="Appccelerate">
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
    using System.Collections.Generic;
    using Appccelerate.EventBroker.Handlers;
    using FluentAssertions;
    using Xbehave;

    public class SubscribersWithoutSenderAndUnwrappedEventArgsSpecifications
    {
        private const string Value = "value";

        private EventBroker eventBroker;
        private Publisher publisher;

        [Background]
        public void SetupEventBroker()
        {
            "Establish an event broker".x(() =>
                this.eventBroker = new EventBroker());

            "Establish an registered publisher".x(() =>
            {
                this.publisher = new Publisher();
                this.eventBroker.Register(this.publisher);
            });
        }

        [Scenario]
        public void RegisterUnregister(
            SubscriberWithoutSenderAndUnwrappedEventArgs subscriber)
        {
            "Establish a subscriber".x(() =>
                subscriber = new SubscriberWithoutSenderAndUnwrappedEventArgs());

            "When registering, firing an event, unregistering and firing another event".x(() =>
            {
                this.eventBroker.Register(subscriber);

                this.publisher.FireEvent(Value);

                this.eventBroker.Unregister(subscriber);

                this.publisher.FireEvent(Value);
            });

            "It should call the handler method on the subscriber with the value of the generic event arguments from the publisher".x(() =>
                subscriber.ReceivedEventArgValues.Should().Contain(Value));

            "It should call the handler method only as long as the subscriber is registered".x(() =>
                subscriber.ReceivedEventArgValues.Should().HaveCount(1, "event should not be routed anymore after subscriber is unregistered."));
        }

        [Scenario]
        public void RegisterUnregisterWithoutAttribute(
            SubscriberWithoutRegistrationAttributeAndSenderAndUnwrappedEventArgs subscriber)
        {
            "Establish a subscriber".x(() =>
                subscriber = new SubscriberWithoutRegistrationAttributeAndSenderAndUnwrappedEventArgs());

            "When registering, firing an event, unregistering and firing another event".x(() =>
            {
                this.eventBroker.SpecialCasesRegistrar.AddSubscription<string>(SimpleEvent.EventTopic, subscriber, subscriber.Handle, new OnPublisher());

                this.publisher.FireEvent(Value);

                this.eventBroker.SpecialCasesRegistrar.RemoveSubscription<string>(SimpleEvent.EventTopic, subscriber, subscriber.Handle);

                this.publisher.FireEvent(Value);
            });

            "It should call the handler method on the subscriber with the value of the generic event arguments from the publisher".x(() =>
                subscriber.ReceivedEventArgValues.Should().Contain(Value));

            "It should call the handler method only as long as the subscriber is registered".x(() =>
                subscriber.ReceivedEventArgValues.Should().HaveCount(1, "event should not be routed anymore after subscriber is unregistered."));
        }

        public class SubscriberWithoutSenderAndUnwrappedEventArgs : SubscriberWithoutSenderAndUnwrappedEventArgsBase
        {
            [EventSubscription(SimpleEvent.EventTopic, typeof(OnPublisher))]
            public void Handle(string value)
            {
                this.ReceivedEventArgValues.Add(value);
            }
        }

        public class SubscriberWithoutRegistrationAttributeAndSenderAndUnwrappedEventArgs : SubscriberWithoutSenderAndUnwrappedEventArgsBase
        {
            public void Handle(string value)
            {
                this.ReceivedEventArgValues.Add(value);
            }
        }

        public class SubscriberWithoutSenderAndUnwrappedEventArgsBase
        {
            public SubscriberWithoutSenderAndUnwrappedEventArgsBase()
            {
                this.ReceivedEventArgValues = new List<object>();
            }

            public List<object> ReceivedEventArgValues { get; private set; }
        }
    }
}