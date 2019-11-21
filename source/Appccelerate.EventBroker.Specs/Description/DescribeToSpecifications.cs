//-------------------------------------------------------------------------------
// <copyright file="DescribeToSpecifications.cs" company="Appccelerate">
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

namespace Appccelerate.EventBroker.Description
{
    using System;
    using System.Globalization;
    using System.IO;
    using Appccelerate.EventBroker.Matchers.Scope;
    using FluentAssertions;

    public class DescribeToSpecifications
    {
        private const string SimpleEventTopic = "topic://SimpleEvent";
        private const string CustomEventTopic = "topic://CustomEvent";

        [Scenario]
        public void Describe(EventBroker eventBroker, StringWriter writer, string description)
        {
            "Establish an event broker with publisher and subscriber".x(() =>
            {
                eventBroker = new EventBroker();

                var publisher = new Publisher();
                var namedPublisher = new NamedPublisher();
                var subscriber = new Subscriber();
                var namedSubscriber = new NamedSubscriber();

                eventBroker.Register(publisher);
                eventBroker.Register(namedPublisher);
                eventBroker.Register(subscriber);
                eventBroker.Register(namedSubscriber);
            });

            "Establish a string writer".x(() =>
                writer = new StringWriter());

            "When writing description to".x(() =>
            {
                eventBroker.DescribeTo(writer);
                description = writer.ToString();
            });

            "It should lists all event topics".x(() =>
                description
                    .Should().Contain(SimpleEventTopic)
                    .And.Contain(CustomEventTopic));

            "it should lists all publishers per event topics".x(() =>
                description
                    .Should().Match("*" + SimpleEventTopic + "*" + typeof(Publisher) + "*" + CustomEventTopic + "*")
                    .And.Match("*" + SimpleEventTopic + "*" + typeof(NamedPublisher) + "*" + CustomEventTopic + "*")
                    .And.Match("*" + CustomEventTopic + "*" + typeof(Publisher) + "*"));

            "It should lists all subscribers per event topic".x(() =>
                    description
                    .Should().Match("*" + SimpleEventTopic + "*" + typeof(Subscriber) + "*" + CustomEventTopic + "*")
                    .And.Match("*" + CustomEventTopic + "*" + typeof(NamedSubscriber) + "*"));

            "It should list the event name per publisher".x(() =>
                description
                .Should().Match(
                    "*" +
                    typeof(Publisher) +
                    "*Event = SimpleEvent*" +
                    typeof(NamedPublisher) +
                    "*Event = SimpleEvent*" +
                    typeof(Publisher) +
                    "*Event = CustomEvent*"));

            "It should list event handler type per publisher".x(() =>
                description
                    .Should().Match(
                        "*" +
                        typeof(Publisher) +
                        "*EventHandler type = System.EventHandler*" +
                        typeof(NamedPublisher) +
                        "*EventHandler type = System.EventHandler*" +
                        typeof(Publisher) +
                        "*EventHandler type = System.EventHandler<Appccelerate.EventBroker.Description.DescribeToSpecifications+CustomEventArgs>*"));

            "It should list matchers per publisher and subscriber".x(() =>
                description
                    .Should().Match(
                        "*" +
                        typeof(NamedPublisher) +
                        "*matchers = subscriber name starts with publisher name*" +
                        typeof(Subscriber) +
                        "*matchers = always*"));

            "It should list the name of named publishers and subscribers".x(() =>
                description
                    .Should().Match(
                        "*" +
                        typeof(NamedPublisher) +
                        "*Name = NamedCustomEventPublisherName*" +
                        typeof(NamedSubscriber) +
                        "*Name = NamedSubscriberName*"));

            "And it lists handler method per subscriber".x(() => description
                .Should().Match(
                    "*" +
                    typeof(Subscriber) +
                    "*Handler method = HandleSimpleEvent*" +
                    typeof(NamedSubscriber) +
                    "*Handler method = HandleCustomEvent*"));

            "And it lists handler type per subscriber".x(() => description
                .Should().Match(
                    "*" +
                    typeof(Subscriber) +
                    "*Handler = Appccelerate.EventBroker.Handlers.OnPublisher*" +
                    typeof(NamedSubscriber) +
                    "*Handler = Appccelerate.EventBroker.Handlers.OnPublisher*"));

            "And it lists event args type per subscriber".x(() => description
                .Should().Match(
                    "*" +
                    typeof(Subscriber) +
                    "*EventArgs type = System.EventArgs*" +
                    typeof(NamedSubscriber) +
                    "*EventArgs type = Appccelerate.EventBroker.Description.DescribeToSpecifications+CustomEventArgs, *"));
        }

        private class Publisher
        {
            [EventPublication(SimpleEventTopic)]
            public event EventHandler SimpleEvent;

            [EventPublication(CustomEventTopic)]
            public event EventHandler<CustomEventArgs> CustomEvent;

            public void FireSimpleEvent() =>
                this.SimpleEvent?.Invoke(this, EventArgs.Empty);

            public void FireCustomEvent() =>
                this.CustomEvent?.Invoke(this, null);
        }

        private class NamedPublisher : INamedItem
        {
            [EventPublication("topic://SimpleEvent", typeof(PublishToChildren))]
            public event EventHandler SimpleEvent;

            public string EventBrokerItemName => "NamedCustomEventPublisherName";

            public void FireSimpleEvent() =>
                this.SimpleEvent?.Invoke(this, EventArgs.Empty);
        }

        private class Subscriber
        {
            [EventSubscription("topic://SimpleEvent", typeof(Handlers.OnPublisher), typeof(SubscribeGlobal))]
            public void HandleSimpleEvent(object sender, EventArgs e)
            {
            }
        }

        private class NamedSubscriber : INamedItem
        {
            public string EventBrokerItemName => "NamedSubscriberName";

            [EventSubscription("topic://CustomEvent", typeof(Handlers.OnPublisher))]
            public void HandleCustomEvent(object sender, CustomEventArgs e)
            {
            }
        }

        private class CustomEventArgs : EventArgs
        {
            public int I { get; } = 5;

            public override string ToString() =>
                this.I.ToString(CultureInfo.InvariantCulture);
        }
    }
}