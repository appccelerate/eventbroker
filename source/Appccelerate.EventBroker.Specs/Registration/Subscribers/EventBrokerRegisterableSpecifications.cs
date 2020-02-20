//-------------------------------------------------------------------------------
// <copyright file="EventBrokerRegisterableSpecifications.cs" company="Appccelerate">
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

namespace Appccelerate.EventBroker.Registration.Subscribers
{
    using FluentAssertions;
    using Xbehave;

    public class EventBrokerRegisterableSpecifications
    {
        [Scenario]
        public void RegisterUnregister(
            EventBroker eventBroker,
            EventBrokerRegisterableSubscriber subscriber,
            bool registerWasCalled,
            bool unregisterWasCalled)
        {
            "Establish an event broker and a subscriber".x(() =>
            {
                eventBroker = new EventBroker();
                subscriber = new EventBrokerRegisterableSubscriber();
            });

            "When registering and unregistering".x(() =>
            {
                eventBroker.Register(subscriber);
                registerWasCalled = subscriber.RegisterWasCalled;

                eventBroker.Unregister(subscriber);
                unregisterWasCalled = subscriber.UnregisterWasCalled;
            });

            "It should register on subscriber when it is registered on event broker".x(() =>
                registerWasCalled.Should().BeTrue());

            "It should unregister on subscriber when it is unregistered from event broker".x(() =>
                unregisterWasCalled.Should().BeTrue());
        }

        public class EventBrokerRegisterableSubscriber : IEventBrokerRegisterable
        {
            public bool RegisterWasCalled { get; private set; }

            public bool UnregisterWasCalled { get; private set; }

            public void Register(IEventRegistrar eventRegistrar) => this.RegisterWasCalled = true;

            public void Unregister(IEventRegistrar eventRegistrar) => this.UnregisterWasCalled = true;
        }
    }
}