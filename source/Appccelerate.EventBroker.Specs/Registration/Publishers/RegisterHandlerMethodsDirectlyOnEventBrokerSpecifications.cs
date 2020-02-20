//-------------------------------------------------------------------------------
// <copyright file="RegisterHandlerMethodsDirectlyOnEventBrokerSpecifications.cs" company="Appccelerate">
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

namespace Appccelerate.EventBroker.Registration.Publishers
{
    using System;
    using FluentAssertions;
    using Xbehave;

    public class RegisterHandlerMethodsDirectlyOnEventBrokerSpecifications
    {
        [Scenario]
        public void Register(
            EventBroker eventBroker,
            Publisher publisher,
            SimpleEvent.EventSubscriber subscriber,
            EventArgs sentEventArgs)
        {
            "Establish an event broker with one subscriber".x(() =>
            {
                eventBroker = new EventBroker();

                subscriber = new SimpleEvent.EventSubscriber();
                eventBroker.Register(subscriber);

                sentEventArgs = new EventArgs();

                publisher = new Publisher();
            });

            "When registering a publisher, sending event, unregistering publisher and sending event".x(() =>
            {
                eventBroker.SpecialCasesRegistrar.AddPublication(
                    SimpleEvent.EventTopic,
                    publisher,
                    "Event",
                    HandlerRestriction.None);

                publisher.FireEvent();

                eventBroker.SpecialCasesRegistrar.RemovePublication(
                    SimpleEvent.EventTopic,
                    publisher,
                    "Event");

                publisher.FireEvent();
            });

            "It should call subscriber".x(() =>
                subscriber.HandledEvent
                    .Should().BeTrue());

            "It should call subscriber only as long as publisher is registered".x(() =>
                subscriber.CallCount
                    .Should().Be(1, "event should not be relayed after unregister"));
        }

        public class Publisher
        {
            public event EventHandler Event = delegate { };

            public void FireEvent()
            {
                this.Event(this, EventArgs.Empty);
            }
        }
    }
}