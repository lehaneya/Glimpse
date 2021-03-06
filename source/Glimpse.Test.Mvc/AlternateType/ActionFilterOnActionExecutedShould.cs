﻿using System;
using System.Web.Mvc;
using Glimpse.Core.Extensibility;
using Glimpse.Mvc.AlternateType;
using Glimpse.Test.Common;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace Glimpse.Test.Mvc.AlternateType
{
    public class ActionFilterOnActionExecutedShould
    {
        [Fact]
        public void ReturnProperMethodToImplement()
        {
            var impl = new ActionFilter.OnActionExecuted();

            Assert.Equal("OnActionExecuted", impl.MethodToImplement.Name);
        }

        [Theory, AutoMock]
        public void ReturnWhenRuntimePolicyIsOff(IAlternateMethodContext context)
        {
            context.Setup(c => c.RuntimePolicyStrategy).Returns(() => RuntimePolicy.Off);

            var impl = new ActionFilter.OnActionExecuted();

            impl.NewImplementation(context);

            context.Verify(c => c.Proceed());
        }

        [Theory, AutoMock]
        public void PublishMessageWhenExecuted([Frozen] IExecutionTimer timer, ActionExecutedContext argument, IAlternateMethodContext context)
        {
            context.Setup(c => c.Arguments).Returns(new object[] { argument });

            var impl = new ActionFilter.OnActionExecuted();

            impl.NewImplementation(context);

            timer.Verify(t => t.Time(It.IsAny<Action>()));
            context.MessageBroker.Verify(mb => mb.Publish(It.IsAny<ActionFilter.OnActionExecuted.Message>()));
        }
    }
}