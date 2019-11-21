//-------------------------------------------------------------------------------
// <copyright file="PublicationMatchersSpecifications.cs" company="Appccelerate">
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

namespace Appccelerate.EventBroker.Matchers
{
    using FluentAssertions;
    using Xbehave;

    public class PublicationMatchersSpecifications
    {
        [Scenario]
        public void Match(
            EventBroker eventBroker,
            ScopeEvent.NamedPublisher publisher,
            ScopeEvent.NamedSubscriber matchingSubscriber,
            ScopeEvent.NamedSubscriber nonMatchingSubscriber)
        {
            "Establish an event broker with a matcher".x(() =>
                eventBroker = new EventBroker());

            "Establish a registered named publisher".x(() =>
            {
                publisher = new ScopeEvent.NamedPublisher("A.Name");
                eventBroker.Register(publisher);
            });

            "Establish a registered matching subscriber".x(() =>
            {
                matchingSubscriber = new ScopeEvent.NamedSubscriber("A.Name");
                eventBroker.Register(matchingSubscriber);
            });

            "Establish a registered not matching subscriber".x(() =>
            {
                nonMatchingSubscriber = new ScopeEvent.NamedSubscriber("A");
                eventBroker.Register(nonMatchingSubscriber);
            });

            "When firing an event".x(() =>
                publisher.FireEventToChildrenAndSiblings());

            "It should call subscriber method of matching subscriber".x(() =>
                matchingSubscriber.CalledGlobally
                                .Should().BeTrue());

            "It should not call subscriber method of non matching subscriber".x(() =>
                nonMatchingSubscriber.CalledGlobally
                    .Should().BeFalse());

        }
    }
}