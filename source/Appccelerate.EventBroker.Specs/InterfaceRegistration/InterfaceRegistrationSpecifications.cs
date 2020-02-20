//-------------------------------------------------------------------------------
// <copyright file="InterfaceRegistrationSpecifications.cs" company="Appccelerate">
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

namespace Appccelerate.EventBroker.InterfaceRegistration
{
    using System;
    using FluentAssertions;
    using Xbehave;

    public class InterfaceRegistrationSpecifications
    {
        private const string EventTopic = "topic://topic";

        [Scenario]
        public void Register(
            EventBroker eventBroker,
            Publisher publisher,
            Subscriber subscriber,
            EventArgs sentEventArgs)
        {
            "Establish an event broker with one publisher and one subscriber".x(() =>
            {
                eventBroker = new EventBroker();
                publisher = new Publisher();
                subscriber = new Subscriber();

                eventBroker.Register(publisher);
                eventBroker.Register(subscriber);
            });

            "When firing an event".x(() =>
                publisher.FireEvent());

            "It should register the implementing event and method".x(() =>
                subscriber.Handled
                    .Should().BeTrue("event should be handled by subscriber"));
        }

        private interface IPublisher
        {
            [EventPublication(EventTopic)]
            event EventHandler Event;
        }

        private interface ISubscriber
        {
            [EventSubscription(EventTopic, typeof(Handlers.OnPublisher))]
            void Handle(object sender, EventArgs eventArgs);
        }

        public class Publisher : IPublisher
        {
            public event EventHandler Event;

            public void FireEvent()
            {
                this.Event?.Invoke(this, EventArgs.Empty);
            }
        }

        public class Subscriber : ISubscriber
        {
            public bool Handled { get; private set; }

            public void Handle(object sender, EventArgs eventArgs)
            {
                this.Handled = true;
            }
        }
    }
}