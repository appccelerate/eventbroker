//-------------------------------------------------------------------------------
// <copyright file="EventBrokerRegisterablesSpecifications.cs" company="Appccelerate">
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

namespace Appccelerate.EventBroker.EventBrokerRegisterables
{
    using FluentAssertions;
    using Xbehave;

    public class EventBrokerSpecifications
    {
        [Scenario]
        public void Register(
            EventBroker eventBroker,
            Registerable registerable)
        {
            "Given an event broker".x(() =>
                eventBroker = new EventBroker());

            "And a registerable".x(() =>
                registerable = new Registerable());

            "When register".x(() =>
            {
                eventBroker.Register(registerable);
            });

            "It should have registered the registerable".x(() =>
                registerable.WasRegistered
                .Should().BeTrue("register should be called"));
        }

        [Scenario]
        public void Unregister(
            EventBroker eventBroker,
            Registerable registerable)
        {
            "Given an event broker".x(() =>
                eventBroker = new EventBroker());

            "And a registerable".x(() =>
                registerable = new Registerable());

            "When unregister".x(() =>
            {
                eventBroker.Register(registerable);
                eventBroker.Unregister(registerable);
            });

            "It should have unregistered the registerable".x(() => registerable.WasUnregistered
                .Should().BeTrue("unregister should be called"));
        }

        public class Registerable : IEventBrokerRegisterable
        {
            public bool WasRegistered { get; private set; }

            public bool WasUnregistered { get; private set; }

            public void Register(IEventRegistrar eventRegistrar) =>
                this.WasRegistered = true;

            public void Unregister(IEventRegistrar eventRegistrar) =>
                this.WasUnregistered = true;
        }
    }
}