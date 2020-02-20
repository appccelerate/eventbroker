//-------------------------------------------------------------------------------
// <copyright file="UnwrappedEventArgsOnlyDelegateWrapper.cs" company="Appccelerate">
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

namespace Appccelerate.EventBroker.Internals.Subscriptions
{
    using System;
    using System.Reflection;
    using Appccelerate.EventBroker.Events;

    public class UnwrappedEventArgsOnlyDelegateWrapper<T> : DelegateWrapper
    {
        public UnwrappedEventArgsOnlyDelegateWrapper(Type eventArgsType, MethodInfo handlerMethod)
            : base(
                eventArgsType,
                typeof(Action<T>),
                handlerMethod)
        {
        }

        public override void Invoke(object subscriber, object sender, EventArgs e)
        {
            Delegate d = this.CreateSubscriptionDelegate(subscriber);

            d.DynamicInvoke(((EventArgs<T>)e).Value);
        }
    }
}