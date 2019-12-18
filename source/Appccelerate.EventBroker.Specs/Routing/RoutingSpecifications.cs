//-------------------------------------------------------------------------------
// <copyright file="RoutingSpecifications.cs" company="Appccelerate">
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

namespace Appccelerate.EventBroker.Routing
{
    using System;
    using System.ComponentModel;
    using Appccelerate.Events;
    using FluentAssertions;
    using Xbehave;

    public class RoutingSpecifications
    {
        private const string EventTopic = "topic://topic";
        private const string EventTopic1 = "topic://topic1";
        private const string EventTopic2 = "topic://topic2";

        private EventBroker eventBroker;

        [Background]
        public void SetupEventBroker()
        {
            "Establish an event broker".x(() =>
                this.eventBroker = new EventBroker());
        }

        [Scenario]
        public void FireEventOnPublisher(
            SimpleEvent.EventPublisher publisher,
            SimpleEvent.EventSubscriber subscriber,
            EventArgs sentEventArgs)
        {
            "Establish a registered publisher".x(() =>
            {
                publisher = new SimpleEvent.EventPublisher();
                this.eventBroker.Register(publisher);
            });

            "Establish a registered subscriber".x(() =>
            {
                subscriber = new SimpleEvent.EventSubscriber();
                this.eventBroker.Register(subscriber);
            });

            "Establish the event args".x(() =>
                sentEventArgs = new EventArgs());

            "When firing the event".x(() =>
                publisher.FireEvent(sentEventArgs));

            "It should call subscriber".x(() =>
                subscriber.HandledEvent
                    .Should().BeTrue("event should be handled by subscriber"));

            "It should pass event args to subscriber".x(() =>
                subscriber.ReceivedEventArgs
                    .Should().BeSameAs(sentEventArgs));
        }

        [Scenario]
        public void FireEventWithCustomArgsOnPublisher(
            CustomEvent.EventPublisher publisher,
            CustomEvent.EventSubscriber subscriber,
            EventArgs<string> sentEventArgs)
        {
            "Establish a registered publisher".x(() =>
            {
                publisher = new CustomEvent.EventPublisher();
                this.eventBroker.Register(publisher);
            });

            "Establish a registered subscriber".x(() =>
            {
                subscriber = new CustomEvent.EventSubscriber();
                this.eventBroker.Register(subscriber);
            });

            "Establish the event args".x(() =>
                sentEventArgs = new EventArgs<string>("value"));

            "When firing the event".x(() =>
                publisher.FireEvent(sentEventArgs));

            "It should call the subscriber".x(() =>
                subscriber.HandledEvent
                    .Should().BeTrue("event should be handled by subscriber"));

            "It should pass event args to the subscriber".x(() =>
                subscriber.ReceivedEventArgs
                    .Should().BeSameAs(sentEventArgs));
        }

        [Scenario]
        public void FireEventAndSubscriberReturnsResultInArgs(
            Publisher publisher,
            Subscriber subscriber,
            CancelEventArgs cancelEventArgs)
        {
            "Establish a registered publisher".x(() =>
            {
                publisher = new Publisher();
                this.eventBroker.Register(publisher);
            });

            "Establish a registered subscriber".x(() =>
            {
                subscriber = new Subscriber();
                this.eventBroker.Register(subscriber);
            });

            "Establish the event args".x(() =>
                cancelEventArgs = new CancelEventArgs());

            "When firing the event".x(() =>
                publisher.FireEvent(cancelEventArgs));

            "It should return result in event args".x(() =>
                cancelEventArgs.Cancel.Should().BeTrue("result value should be returned to caller"));
        }

        [Scenario]
        public void FireEventWithSeveralSubscriber(
            SimpleEvent.EventPublisher publisher,
            SimpleEvent.EventSubscriber subscriber1,
            SimpleEvent.EventSubscriber subscriber2)
        {
            "Establish a registered publisher".x(() =>
            {
                publisher = new SimpleEvent.EventPublisher();
                this.eventBroker.Register(publisher);
            });

            "Establish two registered subscribers".x(() =>
            {
                subscriber1 = new SimpleEvent.EventSubscriber();
                subscriber2 = new SimpleEvent.EventSubscriber();
                this.eventBroker.Register(subscriber1);
                this.eventBroker.Register(subscriber2);
            });

            "When firing the event".x(() =>
                publisher.FireEvent(EventArgs.Empty));

            "It should call all subscribers".x(() =>
            {
                subscriber1.HandledEvent.Should().BeTrue("subscriber1 should be called");
                subscriber2.HandledEvent.Should().BeTrue("subscriber2 should be called");
            });
        }

        [Scenario]
        public void FireEventWithSeveralPublishersHavingSameEventTopic(
            SimpleEvent.EventPublisher publisher1,
            SimpleEvent.EventPublisher publisher2,
            SimpleEvent.EventSubscriber subscriber)
        {
            "Establish two registered publishers".x(() =>
            {
                publisher1 = new SimpleEvent.EventPublisher();
                publisher2 = new SimpleEvent.EventPublisher();
                this.eventBroker.Register(publisher1);
                this.eventBroker.Register(publisher2);
            });

            "Establish a registered subscriber".x(() =>
            {
                subscriber = new SimpleEvent.EventSubscriber();
                this.eventBroker.Register(subscriber);
            });

            "When firing an event on every publisher".x(() =>
            {
                publisher1.FireEvent(EventArgs.Empty);
                publisher2.FireEvent(EventArgs.Empty);
            });

            "It should call subscriber every time".x(() =>
                subscriber.CallCount.Should().Be(2, "subscriber should be called for every fire"));
        }

        [Scenario]
        public void FireEventBelongingToSeveralEventTopics(
            PublisherMultipleTopics publisher,
            SubscriberMultipleTopics subscriber)
        {
            "Establish a registered publisher".x(() =>
            {
                publisher = new PublisherMultipleTopics();
                this.eventBroker.Register(publisher);
            });

            "Establish a registered subscriber".x(() =>
            {
                subscriber = new SubscriberMultipleTopics();
                this.eventBroker.Register(subscriber);
            });

            "When firing the event".x(() =>
                publisher.FireEvent());

            "It should fire all event topics".x(() =>
            {
                subscriber.HandledEventTopic1.Should().BeTrue("EventTopic1 should be handled");
                subscriber.HandledEventTopic2.Should().BeTrue("EventTopic2 should be handled");
            });
        }

        [Scenario]
        public void FireDifferentEventsHandledBySameHandler(
            PublisherMultipleTopicsMultipleMethods publisher,
            SubscriberHandlesMultipleTopics subscriber)
        {
            "Establish a registered publisher".x(() =>
            {
                publisher = new PublisherMultipleTopicsMultipleMethods();
                this.eventBroker.Register(publisher);
            });

            "Establish a registered subscriber".x(() =>
            {
                subscriber = new SubscriberHandlesMultipleTopics();
                this.eventBroker.Register(subscriber);
            });

            "When firing two different events".x(() =>
            {
                publisher.FireEvent1();
                publisher.FireEvent2();
            });

            "It should handle all registered event topics".x(() =>
                subscriber.CallCount.Should().Be(2));
        }

        public class Publisher
        {
            [EventPublication(EventTopic)]
            public event EventHandler<CancelEventArgs> Event;
            public void FireEvent(CancelEventArgs cancelEventArgs)
            {
                this.Event?.Invoke(this, cancelEventArgs);
            }
        }

        public class Subscriber
        {
            [EventSubscription(EventTopic, typeof(Handlers.OnPublisher))]
            public void Handle(object sender, CancelEventArgs cancelEventArgs)
            {
                cancelEventArgs.Cancel = true;
            }
        }

        public class PublisherMultipleTopics
        {
            [EventPublication(EventTopic1)]
            [EventPublication(EventTopic2)]
            public event EventHandler Event;
            public void FireEvent()
            {
                this.Event?.Invoke(this, EventArgs.Empty);
            }
        }

        public class SubscriberMultipleTopics
        {
            public bool HandledEventTopic1 { get; private set; }
            public bool HandledEventTopic2 { get; private set; }
            [EventSubscription(EventTopic1, typeof(Handlers.OnPublisher))]
            public void HandleEventTopic1(object sender, EventArgs eventArgs)
            {
                this.HandledEventTopic1 = true;
            }
            [EventSubscription(EventTopic2, typeof(Handlers.OnPublisher))]
            public void HandleEventTopic2(object sender, EventArgs eventArgs)
            {
                this.HandledEventTopic2 = true;
            }
        }

        public class PublisherMultipleTopicsMultipleMethods
        {
            [EventPublication(EventTopic1)]
            public event EventHandler Event1;

            [EventPublication(EventTopic2)]
            public event EventHandler Event2;

            public void FireEvent1()
             => this.Event1?.Invoke(this, EventArgs.Empty);

            public void FireEvent2() =>
                this.Event2?.Invoke(this, EventArgs.Empty);
        }

        public class SubscriberHandlesMultipleTopics
        {
            public int CallCount { get; private set; }

            [EventSubscription(EventTopic1, typeof(Handlers.OnPublisher))]
            [EventSubscription(EventTopic2, typeof(Handlers.OnPublisher))]
            public void HandleEventTopic1(object sender, EventArgs eventArgs) => this.CallCount++;
        }
    }
}