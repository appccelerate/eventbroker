//-------------------------------------------------------------------------------
// <copyright file="ExtensionSpecifications.cs" company="Appccelerate">
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

using Xbehave;

namespace Appccelerate.EventBroker.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Appccelerate.EventBroker.Exceptions;
    using Appccelerate.EventBroker.Internals.Inspection;
    using Appccelerate.EventBroker.Matchers;
    using FluentAssertions;
    using Machine.Specifications;

    public class ExtensionSpecifications
    {
        private Extension extension;
        private EventBroker eventBroker;
        private SimpleEvent.EventPublisher publisher;
        private SimpleEvent.EventSubscriber subscriber;
        private EventArgs sentEventArgs;
        private ExceptionSubscriber exceptionSubscriber;

        [Background]
        public void SetupEventBrokerAndExtensions()
        {
            "Establish an extension".x(() =>
                this.extension = new Extension());

            "Establish an event broker".x(() =>
                this.eventBroker = new EventBroker());

            "Establish an publisher".x(() =>
                this.publisher = new SimpleEvent.EventPublisher());

            "Establish a subscriber".x(() =>
                this.subscriber = new SimpleEvent.EventSubscriber());

            "Establish event args".x(() =>
                this.sentEventArgs = new EventArgs());
        }

        [Scenario]
        public void Register()
        {
            "Establish an registered extension".x(() => this.eventBroker.AddExtension(this.extension));

            "When registering a publisher and a subscriber".x(() =>
            {
                this.eventBroker.Register(this.publisher);
                this.eventBroker.Register(this.subscriber);
            });

            "It should call extension when the event topic was created".x(() =>
                this.extension.Log.Should().Contain("CreatedTopic"));

            "It should call extension when the publication was created".x(() =>
                this.extension.Log.Should().Contain("CreatedPublication"));

            "It should call extension when the subscription was created".x(() =>
                this.extension.Log.Should().Contain("CreatedSubscription"));

            "It should call extension when the publication was added".x(() =>
                this.extension.Log.Should().Contain("AddedPublication"));

            "It should call extension when the subscription was added".x(() =>
                this.extension.Log.Should().Contain("AddedSubscription"));

            "It should call extension when item was scanned".x(() =>
                this.extension.Log.Should().Contain("Scanned"));

            "It should call extension when item was registered".x(() =>
                this.extension.Log.Should().Contain("RegisteredItem"));
        }

        [Scenario]
        public void Unregister()
        {
            "Establish an event broker with extensions, one subscriber and one publisher".x(() =>
            {
                this.eventBroker.Register(this.publisher);
                this.eventBroker.Register(this.subscriber);
                this.eventBroker.AddExtension(this.extension);
            });

            "When unregistering the subscriber and publisher".x(() =>
            {
                this.eventBroker.Unregister(this.publisher);
                this.eventBroker.Unregister(this.subscriber);
            });

            "It should call the extension when the publication was removed".x(() =>
                this.extension.Log.Should().Contain("RemovedPublication"));

            "It should call the extension when the publication was removed".x(() =>
                this.extension.Log.Should().Contain("RemovedPublication"));

            "It should call the extension when the subscription was removed".x(() =>
                this.extension.Log.Should().Contain("RemovedSubscription"));

            "It should call the extension when the item was scanned".x(() =>
                this.extension.Log.Should().Contain("Scanned"));

            "It should call the extension when the item was should_call_extension_when_item_was_unregistered".x(() =>
                this.extension.Log.Should().Contain("UnregisteredItem"));
        }

        [Scenario]
        public void FireEvent()
        {
            "Establish an event broker with extensions, one subscriber and one publisher".x(() =>
            {
                this.eventBroker.Register(this.publisher);
                this.eventBroker.Register(this.subscriber);
                this.eventBroker.AddExtension(this.extension);
            });

            "When firing an event".x(() => this.publisher.FireEvent(this.sentEventArgs));

            "It should call the extension when the event is firing".x(() =>
                this.extension.Log.Should().Contain("FiringEvent"));

            "It should call the extension when the event is relaying".x(() =>
                this.extension.Log.Should().Contain("RelayingEvent"));

            "It should call the extension when the event was relayed".x(() =>
                this.extension.Log.Should().Contain("RelayedEvent"));

            "It should call the extension when the event was fired".x(() =>
                this.extension.Log.Should().Contain("FiredEvent"));
        }

        [Scenario]
        public void Disposal()
        {
            "Establish an event broker with a publisher and an extension".x(() =>
            {
                this.eventBroker.Register(new SimpleEvent.EventPublisher());
                this.eventBroker.AddExtension(this.extension);
            });

            "When disposing the event broker".x(() => this.eventBroker.Dispose());

            "It should call the extension when the event topic was disposed".x(() =>
                this.extension.Log.Should().Contain("Disposed"));
        }

        [Scenario]
        public void SkipEvents()
        {
            "Establish an event broker with an extension, a publisher, a subscriber and a matcher".x(() =>
            {
                this.eventBroker.Register(this.publisher);
                this.eventBroker.Register(this.subscriber);

                this.eventBroker.AddGlobalMatcher(new Matcher());

                this.eventBroker.AddExtension(this.extension);
            });

            "When firing an event".x(() => this.publisher.FireEvent(EventArgs.Empty));

            "It should call the extension when the event was skipped".x(() =>
                this.extension.Log.Should().Contain("SkippedEvent"));
        }

        [Scenario]
        public void SubscribeToException()
        {
            "Establish an event broker with an extension, a publisher, an exception subscriber".x(() =>
            {
                this.exceptionSubscriber = new ExceptionSubscriber();

                this.eventBroker.Register(this.publisher);
                this.eventBroker.Register(this.exceptionSubscriber);

                this.eventBroker.AddExtension(this.extension);
            });

            "When firing an event".x(() =>
                Catch.Exception(() => this.publisher.FireEvent(EventArgs.Empty)));

            "It should call the extension when subscriper throws exception".x(() =>
                this.extension.Log.Should().Contain("SubscriberExceptionOccured"));
        }

        private class Matcher : IMatcher
        {
            public bool Match(IPublication publication, ISubscription subscription, EventArgs e) => false;

            public void DescribeTo(TextWriter writer)
            {
            }
        }

        private class ExceptionSubscriber
        {
            [EventSubscription(SimpleEvent.EventTopic, typeof(Handlers.OnPublisher))]
            public void Handle(object sender, EventArgs eventArgs)
            {
                throw new Exception("test");
            }
        }

        private class Extension : IEventBrokerExtension
        {
            private readonly StringBuilder log = new StringBuilder();

            public string Log => this.log.ToString();

            public void FiringEvent(IEventTopicInfo eventTopic, IPublication publication, object sender, EventArgs e) =>
                this.log.AppendLine("FiringEvent");

            public void FiredEvent(IEventTopicInfo eventTopic, IPublication publication, object sender, EventArgs e) =>
                this.log.AppendLine("FiredEvent");

            public void RegisteredItem(object item) =>
                this.log.AppendLine("RegisteredItem");

            public void UnregisteredItem(object item) =>
                this.log.AppendLine("UnregisteredItem");

            public void ScannedInstanceForPublicationsAndSubscriptions(object publisher,
                IEnumerable<PropertyPublicationScanResult> foundPublications,
                IEnumerable<PropertySubscriptionScanResult> foundSubscriptions) =>
                this.log.AppendLine("Scanned");

            public void CreatedTopic(IEventTopicInfo eventTopic) =>
                this.log.AppendLine("CreatedTopic");

            public void CreatedPublication(IEventTopicInfo eventTopic, IPublication publication) =>
                this.log.AppendLine("CreatedPublication");

            public void CreatedSubscription(IEventTopicInfo eventTopic, ISubscription subscription) =>
                this.log.AppendLine("CreatedSubscription");

            public void AddedPublication(IEventTopicInfo eventTopic, IPublication publication) =>
                this.log.AppendLine("AddedPublication");

            public void RemovedPublication(IEventTopicInfo eventTopic, IPublication publication) =>
                this.log.AppendLine("RemovedPublication");

            public void AddedSubscription(IEventTopicInfo eventTopic, ISubscription subscription) =>
                this.log.AppendLine("AddedSubscription");

            public void RemovedSubscription(IEventTopicInfo eventTopic, ISubscription subscription) =>
                this.log.AppendLine("RemovedSubscription");

            public void Disposed(IEventTopicInfo eventTopic) => this.log.AppendLine("Disposed");

            public void SubscriberExceptionOccurred(
                IEventTopicInfo eventTopic,
                Exception exception,
                ExceptionHandlingContext context) =>
                this.log.AppendLine("SubscriberExceptionOccured");

            public void RelayingEvent(
                IEventTopicInfo eventTopic,
                IPublication publication,
                ISubscription subscription,
                IHandler handler,
                object sender,
                EventArgs e) =>
                this.log.AppendLine("RelayingEvent");

            public void RelayedEvent(
                IEventTopicInfo eventTopic,
                IPublication publication,
                ISubscription subscription,
                IHandler handler,
                object sender,
                EventArgs e) =>
                this.log.AppendLine("RelayedEvent");

            public void SkippedEvent(
                IEventTopicInfo eventTopic,
                IPublication publication,
                ISubscription subscription,
                object sender,
                EventArgs e) =>
                this.log.AppendLine("SkippedEvent");
        }
    }
}