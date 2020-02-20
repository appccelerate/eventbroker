//-------------------------------------------------------------------------------
// <copyright file="HandlerRestrictionsSpecifications.cs" company="Appccelerate">
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

using Xbehave;

namespace Appccelerate.EventBroker.HandlerRestrictions
{
    using System;
    using Appccelerate.EventBroker.Internals.Exceptions;
    using FluentAssertions;
    using Machine.Specifications;

    public class HandlerRestrictionSpecifications
    {
        private EventBroker eventBroker;
        private HandlerRestrictionEvent.SynchronousSubscriber subscriber;

        [Background]
        public void SetupEventBrokerAndEventSubscriber()
        {
            "Establish an event broker".x(() =>
                this.eventBroker = new EventBroker());

            "Establish a registered synchronous event subscriber".x(() =>
            {
                this.subscriber = new HandlerRestrictionEvent.SynchronousSubscriber();
                this.eventBroker.Register(this.subscriber);
            });
        }

        [Scenario]
        public void FireEventWithSubscriber()
        {
            "When firing an event".x(() =>
                this.eventBroker.Fire(
                    HandlerRestrictionEvent.EventTopic,
                    new object(),
                    HandlerRestriction.Synchronous,
                    new object(),
                    EventArgs.Empty));

            "It should call the subscriber".x(() =>
                this.subscriber.HandledEvent
                    .Should().BeTrue());
        }

        [Scenario]
        public void FireEventWithoutSubscriber(
            Exception exception)
        {
            "When description".x(() =>
                exception = Catch.Exception(() => this.eventBroker.Fire(HandlerRestrictionEvent.EventTopic,
                    new object(), HandlerRestriction.Asynchronous,
                    new object(), EventArgs.Empty)));

            "It should throw an exception".x(() =>
                exception.Should().BeOfType<EventTopicException>());
        }
    }
}