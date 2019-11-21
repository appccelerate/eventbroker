//-------------------------------------------------------------------------------
// <copyright file="ScopeSpecifications.cs" company="Appccelerate">
//   Copyright (c) 2008-2015
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS of the ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------

namespace Appccelerate.EventBroker.Scoping
{
    using FluentAssertions;
    using Xbehave;

    public class ScopeSpecifications
    {
        private EventBroker eventBroker;
        private ScopeEvent.NamedPublisher publisher;
        private ScopeEvent.NamedSubscriber subscriberParent;
        private ScopeEvent.NamedSubscriber subscriberTwin;
        private ScopeEvent.NamedSubscriber subscriberSibling;
        private ScopeEvent.NamedSubscriber subscriberChild;

        [Background]
        public void SetupEventBroker()
        {
            "Establish an event broker".x(() =>
                this.eventBroker = new EventBroker());

            "Establish a registered named publisher".x(() =>
            {
                this.publisher = new ScopeEvent.NamedPublisher("Test.One");
                this.eventBroker.Register(this.publisher);
            });

            "Establish a registered named subscriber parent".x(() =>
            {
                this.subscriberParent = new ScopeEvent.NamedSubscriber("Test");
                this.eventBroker.Register(this.subscriberParent);
            });

            "Establish a registered named subscriber twin".x(() =>
            {
                this.subscriberTwin = new ScopeEvent.NamedSubscriber("Test.One");
                this.eventBroker.Register(this.subscriberTwin);
            });

            "Establish a registered named subscriber sibling".x(() =>
            {
                this.subscriberSibling = new ScopeEvent.NamedSubscriber("Test.Two");
                this.eventBroker.Register(this.subscriberSibling);
            });

            "Establish a registered named subscriber child".x(() =>
            {
                this.subscriberChild = new ScopeEvent.NamedSubscriber("Test.One.Child");
                this.eventBroker.Register(this.subscriberChild);
            });
        }

        [Scenario]
        public void GlobalEvents()
        {
            "When firing a global event".x(() =>
                this.publisher.FireEventGlobally());

            "It should be handled by the global handler of the parent".x(() =>
                this.subscriberParent.CalledGlobally.Should().BeTrue());

            "It should be handled by the child handler of the parent".x(() =>
                this.subscriberParent.CalledFromChild.Should().BeTrue());

            "It should not be handled by the parent handler of the parent".x(() =>
                this.subscriberParent.CalledFromParent.Should().BeFalse());

            "It should be handled by the global handler of the twin".x(() =>
                this.subscriberTwin.CalledGlobally.Should().BeTrue());

            "It should be handled by the child handler of the twin".x(() =>
                this.subscriberTwin.CalledFromChild.Should().BeTrue());

            "It should be handled by the parent handler of the twin".x(() =>
                this.subscriberTwin.CalledFromParent.Should().BeTrue());

            "It should be handled by the global handler of the sibling".x(() =>
                this.subscriberSibling.CalledGlobally.Should().BeTrue());

            "It should not be handled by the child handler of the sibling".x(() =>
                this.subscriberSibling.CalledFromChild.Should().BeFalse());

            "It should not be handled by the parent handler of the sibling".x(() =>
                this.subscriberSibling.CalledFromParent.Should().BeFalse());

            "It should be handled by the global handler of the child".x(() =>
                this.subscriberChild.CalledGlobally.Should().BeTrue());

            "It should not be handled by the child handler of the child".x(() =>
                this.subscriberChild.CalledFromChild.Should().BeFalse());

            "It should be handled by the parent handler of the child".x(() =>
                this.subscriberChild.CalledFromParent.Should().BeTrue());
        }

        [Scenario]
        public void ParentEvents()
        {
            "When firing a parent event".x(() =>
                this.publisher.FireEventToParentsAndSiblings());

            "It should be handled by the global handler of the parent".x(() =>
                this.subscriberParent.CalledGlobally.Should().BeTrue());

            "It should be handled by the child handler of the parent".x(() =>
                this.subscriberParent.CalledFromChild.Should().BeTrue());

            "It should not be handled by the parent handler of the parent".x(() =>
                this.subscriberParent.CalledFromParent.Should().BeFalse());

            "It should be handled by the global handler of the twin".x(() =>
                this.subscriberTwin.CalledGlobally.Should().BeTrue());

            "It should be handled by the child handler of the twin".x(() =>
                this.subscriberTwin.CalledFromChild.Should().BeTrue());

            "It should be handled by the parent handler of the twin".x(() =>
                this.subscriberTwin.CalledFromParent.Should().BeTrue());

            "It should not be handled by the global handler of the sibling".x(() =>
                this.subscriberSibling.CalledGlobally.Should().BeFalse());

            "It should not be handled by the child handler of the sibling".x(() =>
                this.subscriberSibling.CalledFromChild.Should().BeFalse());

            "It should not be handled by the parent handler of the sibling".x(() =>
                this.subscriberSibling.CalledFromParent.Should().BeFalse());

            "It should not be handled by the global handler of the child".x(() =>
                this.subscriberChild.CalledGlobally.Should().BeFalse());

            "It should not be handled by the child handler of the child".x(() =>
                this.subscriberChild.CalledFromChild.Should().BeFalse());

            "It should not be handled by the parent handler of the child".x(() =>
                this.subscriberChild.CalledFromParent.Should().BeFalse());
        }

        [Scenario]
        public void ChildrenEvents()
        {
            "When firing a child event".x(() =>
                this.publisher.FireEventToChildrenAndSiblings());

            "It should not be handled by the global handler of the parent".x(() =>
                this.subscriberParent.CalledGlobally.Should().BeFalse());

            "It should not be handled by the child handler of the parent".x(() =>
                this.subscriberParent.CalledFromChild.Should().BeFalse());

            "It should not be handled by the parent handler of the parent".x(() =>
                this.subscriberParent.CalledFromParent.Should().BeFalse());

            "It should be handled by the global handler of the twin".x(() =>
                this.subscriberTwin.CalledGlobally.Should().BeTrue());

            "It should be handled by the child handler of the twin".x(() =>
                this.subscriberTwin.CalledFromChild.Should().BeTrue());

            "It should be handled by the parent handler of the twin".x(() =>
                this.subscriberTwin.CalledFromParent.Should().BeTrue());

            "It should not be handled by the global handler of the sibling".x(() =>
                this.subscriberSibling.CalledGlobally.Should().BeFalse());

            "It should not be handled by the child handler of the sibling".x(() =>
                this.subscriberSibling.CalledFromChild.Should().BeFalse());

            "It should not be handled by the parent handler of the sibling".x(() =>
                this.subscriberSibling.CalledFromParent.Should().BeFalse());

            "It should be handled by the global handler of the child".x(() =>
                this.subscriberChild.CalledGlobally.Should().BeTrue());

            "It should not be handled by the child handler of the child".x(() =>
                this.subscriberChild.CalledFromChild.Should().BeFalse());

            "It should be handled by the parent handler of the child".x(() =>
                this.subscriberChild.CalledFromParent.Should().BeTrue());
        }
    }
}