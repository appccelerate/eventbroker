//-------------------------------------------------------------------------------
// <copyright file="SubscriberExceptionsSpecifications.cs" company="Appccelerate">
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
    using Appccelerate.EventBroker.Exceptions;
    using Appccelerate.EventBroker.Extensions;
    using FluentAssertions;
    using Machine.Specifications;
    using Xbehave;

    public class SubscriberExceptionsSpecifications
    {
        private EventBroker eventBroker;

        [Background]
        public void SetupEventBroker()
        {
            "Establish an event broker".x(() =>
                this.eventBroker = new EventBroker());
        }

        [Scenario]
        public void HandleThrowingSubscription(
            Exception exception,
            SimpleEvent.EventPublisher publisher,
            ThrowingSubscriber subscriber)
        {
            "Establish a registered publisher and throwing subscriber".x(() =>
            {
                publisher = new SimpleEvent.EventPublisher();
                subscriber = new ThrowingSubscriber();

                this.eventBroker.Register(publisher);
                this.eventBroker.Register(subscriber);
            });

            "When firing an event".x(() =>
                exception = Catch.Exception(() => publisher.FireEvent(EventArgs.Empty)));

            "It should pass the exception to the publisher".x(() =>
                exception
                    .Should().NotBeNull()
                    .And.Subject.As<Exception>().Message
                    .Should().Be("test"));
        }

        [Scenario]
        public void HandleMultipleThrowingSubscriptions(
            Exception exception,
            SimpleEvent.EventPublisher publisher,
            ThrowingSubscriber subscriber1,
            ThrowingSubscriber subscriber2)
        {
            "Establish a registered publisher and throwing subscriber".x(() =>
            {
                publisher = new SimpleEvent.EventPublisher();
                subscriber1 = new ThrowingSubscriber();
                subscriber2 = new ThrowingSubscriber();

                this.eventBroker.Register(publisher);
                this.eventBroker.Register(subscriber1);
                this.eventBroker.Register(subscriber2);
            });

            "When firing an event".x(() =>
                exception = Catch.Exception(() => publisher.FireEvent(EventArgs.Empty)));

            "It should pass the exception to the publisher".x(() =>
                exception
                    .Should().NotBeNull()
                    .And.Subject.As<Exception>().Message
                    .Should().Be("test"));

            "It should stop executing subscribers after the first exception".x(() =>
                subscriber2.Handled.Should().BeFalse("second subscriber should not be clled anymore"));
        }

        [Scenario]
        public void HandleThrowingSubscriptionWithExtension(
            Exception exception,
            SimpleEvent.EventPublisher publisher,
            ThrowingSubscriber subscriber)
        {
            "Establish a registered publisher and throwing subscriber".x(() =>
            {
                publisher = new SimpleEvent.EventPublisher();
                subscriber = new ThrowingSubscriber();

                this.eventBroker.Register(publisher);
                this.eventBroker.Register(subscriber);

                this.eventBroker.AddExtension(new ExceptionHandlingExtension());
            });

            "When firing an event".x(() =>
                exception = Catch.Exception(() => publisher.FireEvent(EventArgs.Empty)));

            "It should not pass exception to the publisher".x(() =>
                exception
                    .Should().BeNull());
        }

        public class ThrowingSubscriber
        {
            public bool Handled { get; private set; }

            [EventSubscription(SimpleEvent.EventTopic, typeof(Handlers.OnPublisher))]
            public void Handle(object sender, EventArgs eventArgs)
            {
                this.Handled = true;
                throw new Exception("test");
            }
        }

        private class ExceptionHandlingExtension : EventBrokerExtensionBase
        {
            public override void SubscriberExceptionOccurred(IEventTopicInfo eventTopic, Exception exception, ExceptionHandlingContext context)
            {
                context.SetHandled();
            }
        }
    }
}