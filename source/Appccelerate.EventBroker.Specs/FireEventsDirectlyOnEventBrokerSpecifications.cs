//-------------------------------------------------------------------------------
// <copyright file="FireEventsDirectlyOnEventBrokerSpecifications.cs" company="Appccelerate">
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

namespace Appccelerate.EventBroker
{
    using System;
    using FluentAssertions;
    using Xbehave;

    public class FireEventsDirectlyOnEventBrokerSpecifications
    {
        private const string EventTopic = "topic://topic";

        [Scenario]
        public void DirectEventBrokerEventPublication(
            EventBroker eventBroker,
            Subscriber subscriber)
        {
            "Establish an event broker".x(() =>
                eventBroker = new EventBroker());

            "Establish a registered subscriber".x(() =>
            {
                subscriber = new Subscriber();
                eventBroker.Register(subscriber);
            });

            "When firing event directly on event broker".x(() =>
                eventBroker.Fire(EventTopic, new object(), HandlerRestriction.None, new object(), EventArgs.Empty));

            "It should call the subscriber".x(() =>
                subscriber.HandledEvent
                    .Should().BeTrue());
        }

        public class Subscriber
        {
            public bool HandledEvent { get; private set; }

            [EventSubscription(EventTopic, typeof(Handlers.OnPublisher))]
            public void Handle(object sender, EventArgs eventArgs)
            {
                this.HandledEvent = true;
            }
        }
    }
}