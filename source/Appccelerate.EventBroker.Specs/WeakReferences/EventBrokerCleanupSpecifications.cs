//-------------------------------------------------------------------------------
// <copyright file="EventBrokerCleanupSpecifications.cs" company="Appccelerate">
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

namespace Appccelerate.EventBroker.WeakReferences
{
    using System;
    using FluentAssertions;
    using Xbehave;

    public class EventBrokerCleanupSpecifications
    {
        [Scenario]
        public void SubscriberCleanup(
            EventBroker eventBroker,
            SimpleEvent.EventPublisher publisher,
            SimpleEvent.EventSubscriber subscriber,
            SimpleEvent.EventSubscriber subscriberThatIsCollected,
            SimpleEvent.RegisterableEventSubscriber registerableSubscriberThatIsCollected,
            WeakReference referenceOnSubscriberThatIsCollected,
            WeakReference referenceOnRegisterableSubscriberThatIsCollected)
        {
            "Establish an event broker".x(() =>
                eventBroker = new EventBroker());

            "Establish a registered publisher".x(() =>
            {
                publisher = new SimpleEvent.EventPublisher();
                eventBroker.Register(publisher);
            });

            "Establish a registered subscriber".x(() =>
            {
                subscriber = new SimpleEvent.EventSubscriber();
                eventBroker.Register(subscriber);
            });

            "Establish a registered subscriber that is collected".x(() =>
            {
                subscriberThatIsCollected = new SimpleEvent.EventSubscriber();
                eventBroker.Register(subscriberThatIsCollected);
            });

            "Establish a registerable subscriber that is collected".x(() =>
            {
                registerableSubscriberThatIsCollected = new SimpleEvent.RegisterableEventSubscriber();
                eventBroker.Register(registerableSubscriberThatIsCollected);
            });

            "Establish a weak references to the collected subscribers".x(() =>
            {
                referenceOnSubscriberThatIsCollected = new WeakReference(subscriberThatIsCollected);
                referenceOnRegisterableSubscriberThatIsCollected =
                    new WeakReference(registerableSubscriberThatIsCollected);
            });

            "When enforcing the garbage collection".x(() =>
            {
                subscriberThatIsCollected = null;
                registerableSubscriberThatIsCollected = null;
                GC.Collect();
            });

            "It should garbage collect the subscribers registered by attribute".x(() =>
                referenceOnSubscriberThatIsCollected.IsAlive
                    .Should().BeFalse("subscriber should be collected"));

            "It should garbage collect the subscribers registered by registrar".x(() =>
                referenceOnRegisterableSubscriberThatIsCollected.IsAlive
                    .Should().BeFalse("subscriber should be collected"));
        }

        [Scenario]
        public void PublisherCleanup(
            EventBroker eventBroker,
            SimpleEvent.EventPublisher publisher,
            SimpleEvent.RegisterableEventPublisher registerablePublisher,
            SimpleEvent.EventSubscriber subscriber,
            WeakReference weakReferenceOnPublisher,
            WeakReference weakReferenceOnRegisterablePublisher)
        {
            "Establish an event broker".x(() =>
                eventBroker = new EventBroker());

            "Establish a registered publisher".x(() =>
            {
                publisher = new SimpleEvent.EventPublisher();
                eventBroker.Register(publisher);
            });

            "Establish a registered registerable publisher".x(() =>
            {
                registerablePublisher = new SimpleEvent.RegisterableEventPublisher();
                eventBroker.Register(registerablePublisher);
            });

            "Establish a registered subscriber".x(() =>
            {
                subscriber = new SimpleEvent.EventSubscriber();
                eventBroker.Register(subscriber);
            });

            "Establish a weak references to the collected subscribers".x(() =>
            {
                weakReferenceOnPublisher = new WeakReference(publisher);
                weakReferenceOnRegisterablePublisher = new WeakReference(registerablePublisher);
            });

            "When enforcing the garbage collection".x(() =>
            {
                publisher = null;
                registerablePublisher = null;
                GC.Collect();
            });

            "It should garbage collect publisher registered by property".x(() =>
                weakReferenceOnPublisher.IsAlive
                    .Should().BeFalse("publisher should be collected"));

            "It should garbage collect subscribers registered by registrar".x(() =>
                weakReferenceOnRegisterablePublisher.IsAlive
                    .Should().BeFalse("publisher should be collected"));
        }

        [Scenario]
        public void EventBrokerCleanup(
            EventBroker eventBroker,
            WeakReference publisher,
            WeakReference registerablePublisher,
            WeakReference subscriber,
            WeakReference registerableSubscriber)
        {
            "Establish an event broker".x(() =>
                eventBroker = new EventBroker());

            "Establish a registered publisher".x(() =>
            {
                var p = new SimpleEvent.EventPublisher();
                publisher = new WeakReference(p);
                eventBroker.Register(p);
            });

            "Establish a registered registerable publisher".x(() =>
            {
                var p = new SimpleEvent.RegisterableEventPublisher();
                registerablePublisher = new WeakReference(p);
                eventBroker.Register(p);
            });

            "Establish a registered subscriber".x(() =>
            {
                var s = new SimpleEvent.EventSubscriber();
                subscriber = new WeakReference(s);
                eventBroker.Register(s);
            });

            "Establish a registered registerable subscriber".x(() =>
            {
                var s = new SimpleEvent.RegisterableEventSubscriber();
                registerableSubscriber = new WeakReference(s);
                eventBroker.Register(s);
            });

            "When enforcing the garbage collection".x(
                GC.Collect);

            "It should unregister publishers registered by attribute".x(() =>
                publisher.IsAlive
                    .Should().BeFalse("publisher should not be referenced anymore"));

            "It should unregister publishers registered by registrar".x(() =>
                registerablePublisher.IsAlive
                    .Should().BeFalse("publisher should not be referenced anymore"));

            "It should unregister subscribers registered by attribtue".x(() =>
                subscriber.IsAlive
                    .Should().BeFalse("subscriber should not be referenced anymore"));

            "It should unregister subscribers registered by registrar".x(() =>
                registerableSubscriber.IsAlive
                    .Should().BeFalse("subscriber should not be referenced anymore"));
        }
    }
}