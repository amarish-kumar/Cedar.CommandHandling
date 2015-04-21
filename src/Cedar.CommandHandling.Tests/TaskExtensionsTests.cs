﻿namespace Cedar.CommandHandling
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;

    public class TaskExtensionsTests
    {
        [Fact]
        public void Should_timeout()
        {
            Func<Task> act = () => Task.Delay(10000).WithTimeout(TimeSpan.FromMilliseconds(1));

            act.ShouldThrow<TimeoutException>();
        }
    }

}