//-------------------------------------------------------------------------------
// <copyright file="RegistrationExceptionsSpecifications.cs" company="Appccelerate">
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

namespace Appccelerate.EventBroker.Registration
{
    using System;
    using System.Threading;
    using Appccelerate.EventBroker.Handlers;
    using Appccelerate.EventBroker.Internals.Exceptions;
    using Appccelerate.Events;
    using FluentAssertions;
    using Machine.Specifications;
    using Xbehave;

    public class RegistrationExceptionsSpecifications
    {
        [Scenario]
        public void MultipleRegistrationOnSameTopic(
            EventBroker eventBroker,
            Exception exception)
        {
            "Establish an event broker".x(() =>
                eventBroker = new EventBroker());

            "When registering an invalid publisher with multiple publications on same event topic".x(() =>
                exception = Catch.Exception(() =>
                    eventBroker.Register(new InvalidPublisherWithRepeatedEventPublication())));

            "It should throw an exception".x(() =>
                exception
                    .Should().NotBeNull()
                    .And.BeOfType<RepeatedPublicationException>());
        }

        [Scenario]
        public void PublisherRegistrationWithInvalidEventSignature(
            EventBroker eventBroker,
            Exception exception)
        {
            "Establish an event broker".x(() =>
                eventBroker = new EventBroker());

            "When registering an invalid publisher with invalid event signature".x(() =>
                exception = Catch.Exception(() =>
                    eventBroker.Register(new InvalidPublisherWithWrongEventSignature())));

            "It should throw an exception".x(() =>
                exception
                    .Should().NotBeNull()
                    .And.BeOfType<InvalidPublicationSignatureException>());
        }

        [Scenario]
        public void SubscriberRegistrationWithInvalidHandlerMethodSignature(
            EventBroker eventBroker,
            Exception exception)
        {
            "Establish an event broker".x(() =>
                eventBroker = new EventBroker());

            "When registering an invalid subscriber with invalid handler event signature".x(() =>
                exception = Catch.Exception(() =>
                    eventBroker.Register(new InvalidSubscriberWithWrongSignature())));

            "It should throw an exception".x(() =>
                exception
                    .Should().NotBeNull()
                    .And.BeOfType<InvalidSubscriptionSignatureException>());
        }

        [Scenario]
        public void PublisherRegistrationWithStaticPublicationEvent(
            EventBroker eventBroker,
            Exception exception)
        {
            "Establish an event broker".x(() =>
                eventBroker = new EventBroker());

            "When registering an invalid publisher with static event publication event".x(() =>
                exception = Catch.Exception(() =>
                    eventBroker.Register(new InvalidPublisherWithStaticEvent())));

            "It should throw an exception".x(() =>
                exception
                    .Should().NotBeNull()
                    .And.BeOfType<StaticPublisherEventException>());
        }

        [Scenario]
        public void SubscriberRegistrationWithStaticHandlerMethod(
            EventBroker eventBroker,
            Exception exception)
        {
            "Establish an event broker".x(() =>
                eventBroker = new EventBroker());

            "When registering an invalid subscriber with static subscription handler method".x(() =>
                exception = Catch.Exception(() =>
                    eventBroker.Register(new InvalidSubscriberStaticHandler())));

            "It should throw an exception".x(() =>
                exception
                    .Should().NotBeNull()
                    .And.BeOfType<StaticSubscriberHandlerException>());
        }

        [Scenario]
        public void SubscriberRegistrationOnUiThreadWithoutBeingOnUiThread(
            EventBroker eventBroker,
            Exception exception)
        {
            "Establish an event broker".x(() =>
                eventBroker = new EventBroker());

            "When registering an subscriber with handler on ui thread without being on ui thread".x(() =>
            {
                SynchronizationContext.SetSynchronizationContext(null);

                exception = Catch.Exception(() =>
                    eventBroker.Register(new UserInterfaceSubscriber()));
            });

            "It should throw an exception".x(() =>
                exception
                    .Should().NotBeNull()
                    .And.BeOfType<NotUserInterfaceThreadException>());
        }

        [Scenario]
        public void SubscriberRegistrationWithEventArgsNotMatchingPublication(
            EventBroker eventBroker,
            Exception exception,
            SimpleEvent.EventPublisher eventPublisher)
        {
            "Establish an event broker".x(() =>
                eventBroker = new EventBroker());

            "Establish a registered publisher".x(() =>
            {
                eventPublisher = new SimpleEvent.EventPublisher();
                eventBroker.Register(eventPublisher);
            });

            "When registering an subscriber with event args not matching the existing publication".x(() =>
                exception = Catch.Exception(() =>
                    eventBroker.Register(new SubscriberWithWrongEventArgsType())));

            "It should throw an exception".x(() =>
                exception
                    .Should().NotBeNull()
                    .And.BeOfType<EventTopicException>());
        }

        [Scenario]
        public void PublisherRegistrationNotMatchingExistingSubscription(
            EventBroker eventBroker,
            Exception exception,
            CustomEvent.EventSubscriber subscriber)
        {
            "Establish an event broker".x(() =>
                eventBroker = new EventBroker());

            "Establish a registered subscriber".x(() =>
            {
                subscriber = new CustomEvent.EventSubscriber();
                eventBroker.Register(subscriber);
            });

            "When registering an publisher and event args type not matching existing subscription".x(() =>
                exception = Catch.Exception(() =>
                    eventBroker.Register(new PublisherWithWrongEventArgsType())));

            "It should throw an exception".x(() =>
                exception
                    .Should().NotBeNull()
                    .And.BeOfType<EventTopicException>());
        }

        private class InvalidPublisherWithRepeatedEventPublication
        {
            [EventPublication(SimpleEvent.EventTopic)]
            [EventPublication(SimpleEvent.EventTopic)]
            // ReSharper disable once EventNeverInvoked
            public event EventHandler Event;
        }

        private class InvalidPublisherWithWrongEventSignature
        {
            public delegate int MyEventHandler(string name);

            [EventPublication("topic")]
            // ReSharper disable once EventNeverInvoked
            // ReSharper disable once EventNeverSubscribedTo.Global
            public event MyEventHandler SimpleEvent;
        }

        private class InvalidSubscriberWithWrongSignature
        {
            [EventSubscription("topic", typeof(Handlers.OnPublisher))]
            public void SimpleEvent(int i, EventArgs e)
            {
            }
        }

        private class InvalidPublisherWithStaticEvent
        {
            [EventPublication("topic")]
            // ReSharper disable once EventNeverInvoked
            // ReSharper disable once EventNeverSubscribedTo.Global
            public static event EventHandler SimpleEvent;
        }

        private class InvalidSubscriberStaticHandler
        {
            [EventSubscription("topic", typeof(Handlers.OnPublisher))]
            public static void SimpleEvent(object sender, EventArgs e)
            {
            }
        }

        private class UserInterfaceSubscriber
        {
            [EventSubscription("topic", typeof(Handlers.OnUserInterface))]
            public void SimpleEvent(object sender, EventArgs e)
            {
            }
        }

        private class SubscriberWithWrongEventArgsType
        {
            [EventSubscription(SimpleEvent.EventTopic, typeof(Handlers.OnPublisher))]
            public void Handle(object sender, EventArgs<string> e)
            {
            }
        }

        private class PublisherWithWrongEventArgsType
        {
            [EventPublication(CustomEvent.EventTopic)]
            public event EventHandler<EventArgs<int>> Event;
        }
    }
}