using System;
using MyAzureStudies.Library.ServiceBus;
using Xunit;
using Xunit.Abstractions;

namespace MyAzureStudies.Tests
{
    public class ServiceBusTests
    {
        private ITestOutputHelper testOutputHelper { get; }

        public ServiceBusTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact(DisplayName = "Test the default service bus behaviour")]
        public void ShouldReceiveMessage()
        {
            var serviceBus = new Sample("", "", testOutputHelper);

            serviceBus.Execute().GetAwaiter().GetResult();
        }
    }
}
