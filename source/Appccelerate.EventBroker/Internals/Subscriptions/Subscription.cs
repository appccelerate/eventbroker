//-------------------------------------------------------------------------------
// <copyright file="Subscription.cs" company="Appccelerate">
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

namespace Appccelerate.EventBroker.Internals.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Appccelerate.EventBroker.Matchers;

    /// <summary>
    /// Represents a topic subscription.
    /// </summary>
    internal class Subscription : ISubscription
    {
        private readonly WeakReference subscriber;
        private readonly IList<ISubscriptionMatcher> subscriptionMatchers;
        private readonly IHandler handler;
        private readonly IExtensionHost extensionHost;

        private readonly DelegateWrapper delegateWrapper;

        public Subscription(
            object subscriber,
            DelegateWrapper delegateWrapper,
            IHandler handler,
            IList<ISubscriptionMatcher> subscriptionMatchers,
            IExtensionHost extensionHost)
        {
            this.subscriber = new WeakReference(subscriber);
            this.delegateWrapper = delegateWrapper;
            this.subscriptionMatchers = subscriptionMatchers;
            this.handler = handler;
            this.extensionHost = extensionHost;
        }

        public Type EventArgsType => this.delegateWrapper.EventArgsType;

        public object Subscriber => this.subscriber.Target;

        public string HandlerMethodName => this.delegateWrapper.HandlerMethod.Name;

        public IHandler Handler => this.handler;

        public IList<ISubscriptionMatcher> SubscriptionMatchers => this.subscriptionMatchers;

        public EventTopicFireDelegate GetHandler()
        {
            return this.EventTopicFireHandler;
        }

        public void DescribeTo(TextWriter writer)
        {
            Guard.AgainstNullArgument(nameof(writer), writer);

            if (!this.subscriber.IsAlive)
            {
                return;
            }

            writer.Write(this.Subscriber.GetType().FullName);

            var namedItem = this.Subscriber as INamedItem;
            if (namedItem != null)
            {
                writer.Write(", Name = ");
                writer.Write(namedItem.EventBrokerItemName);
            }

            writer.Write(", Handler method = ");
            writer.Write(this.HandlerMethodName);

            writer.Write(", Handler = ");
            writer.Write(this.handler.GetType().FullNameToString());

            writer.Write(", EventArgs type = ");
            writer.Write(this.EventArgsType.FullNameToString());

            writer.Write(", matchers = ");
            foreach (ISubscriptionMatcher subscriptionMatcher in this.subscriptionMatchers)
            {
                subscriptionMatcher.DescribeTo(writer);
                writer.Write(" ");
            }
        }

        private void EventTopicFireHandler(IEventTopicInfo eventTopic, object sender, EventArgs e, IPublication publication)
        {
            var target = this.Subscriber;
            if (target == null)
            {
                return;
            }

            this.extensionHost.ForEach(extension => extension.RelayingEvent(eventTopic, publication, this, this.handler, sender, e));

            this.handler.Handle(eventTopic, target, sender, e, this.delegateWrapper);

            this.extensionHost.ForEach(extension => extension.RelayedEvent(eventTopic, publication, this, this.handler, sender, e));
        }
    }
}