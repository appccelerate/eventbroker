//-------------------------------------------------------------------------------
// <copyright file="SubscribersWithoutSenderAndWithoutEventArgsSpecifications.cs" company="Appccelerate">
//   Copyright (c) 2008-2020
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
    using Appccelerate.EventBroker.Handlers;
    using FluentAssertions;
    using Xbehave;

    public class SubscribersWithoutSenderAndWithoutEventArgsSpecifications
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
            SubscriberWithoutSenderAndWithoutEventArgs subscriber)
        {
            "Establish a subscriber".x(() =>
                subscriber = new SubscriberWithoutSenderAndWithoutEventArgs());

            "When registering, firing an event, unregistering and firing another event".x(() =>
            {
                this.eventBroker.Register(subscriber);

                this.publisher.FireEvent(Value);

                this.eventBroker.Unregister(subscriber);

                this.publisher.FireEvent(Value);
            });

            "It should call the handler method".x(() =>
                subscriber.CallCount.Should().BePositive());

            "It should call the handler method only as long as the subscriber is registered".x(() =>
                subscriber.CallCount.Should().Be(1, "event should not be routed anymore after subscriber is unregistered."));
        }

        [Scenario]
        public void RegisterUnregisterWithoutAttribute(
            SubscriberWithoutRegistrationAttributeAndSenderAndWithoutEventArgs subscriber)
        {
            "Establish a subscriber".x(() =>
                subscriber = new SubscriberWithoutRegistrationAttributeAndSenderAndWithoutEventArgs());

            "When registering, firing an event, unregistering and firing another event".x(() =>
            {
                this.eventBroker.SpecialCasesRegistrar.AddSubscription(SimpleEvent.EventTopic, subscriber, subscriber.Handle, new OnPublisher());

                this.publisher.FireEvent(Value);

                this.eventBroker.SpecialCasesRegistrar.RemoveSubscription(SimpleEvent.EventTopic, subscriber, subscriber.Handle);

                this.publisher.FireEvent(Value);
            });

            "It should call the handler method".x(() =>
                subscriber.CallCount.Should().BePositive());

            "It should call the handler method only as long as the subscriber is registered".x(() =>
                subscriber.CallCount.Should().Be(1, "event should not be routed anymore after subscriber is unregistered."));
        }

        public class SubscriberWithoutSenderAndWithoutEventArgs : SubscriberWithoutSenderAndWithoutEventArgsBase
        {
            [EventSubscription(SimpleEvent.EventTopic, typeof(OnPublisher))]
            public void Handle()
            {
                this.CallCount++;
            }
        }

        public class SubscriberWithoutRegistrationAttributeAndSenderAndWithoutEventArgs : SubscriberWithoutSenderAndWithoutEventArgsBase
        {
            public void Handle()
            {
                this.CallCount++;
            }
        }

        public class SubscriberWithoutSenderAndWithoutEventArgsBase
        {
            protected SubscriberWithoutSenderAndWithoutEventArgsBase() => this.CallCount = 0;

            public int CallCount { get; protected set; }
        }
    }
}