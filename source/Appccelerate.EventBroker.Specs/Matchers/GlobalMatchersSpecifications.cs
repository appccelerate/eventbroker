//-------------------------------------------------------------------------------
// <copyright file="GlobalMatchersSpecifications.cs" company="Appccelerate">
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

namespace Appccelerate.EventBroker.Matchers
{
    using System;
    using System.IO;
    using FluentAssertions;
    using Xbehave;

    public class GlobalMatchersSpecifications
    {
        [Scenario]
        public void Match(
            EventBroker eventBroker,
            SimpleEvent.EventPublisher publisher,
            SimpleEvent.EventSubscriber subscriber)
        {
            "Establish an event broker with a matcher".x(() =>
            {
                eventBroker = new EventBroker();
                eventBroker.AddGlobalMatcher(new Matcher(true));
            });

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

            "When firing an event".x(() =>
                publisher.FireEvent(EventArgs.Empty));

            "It should call subscriber".x(() =>
                subscriber.HandledEvent
                    .Should().BeTrue("matcher should not block call"));
        }

        [Scenario]
        public void NoMatch(
            EventBroker eventBroker,
            SimpleEvent.EventPublisher publisher,
            SimpleEvent.EventSubscriber subscriber)
        {
            "Establish an event broker with a matcher".x(() =>
            {
                eventBroker = new EventBroker();
                eventBroker.AddGlobalMatcher(new Matcher(false));
            });

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

            "When firing an event".x(() =>
                publisher.FireEvent(EventArgs.Empty));

            "It should not call subscriber".x(() =>
                subscriber.HandledEvent
                    .Should().BeFalse("matcher should block call"));
        }

        private class Matcher : IMatcher
        {
            private readonly bool shouldMatch;

            public Matcher(bool shouldMatch) =>
                this.shouldMatch = shouldMatch;

            public bool Match(IPublication publication, ISubscription subscription, EventArgs e) => this.shouldMatch;

            public void DescribeTo(TextWriter writer)
            {
            }
        }
    }
}