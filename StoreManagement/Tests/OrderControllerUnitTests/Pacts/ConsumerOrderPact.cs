using Newtonsoft.Json;
using PactNet;
using PactNet.Mocks.MockHttpService;
using PactNet.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderControllerUnitTests.Pacts
{
    public class ConsumerOrderPact
    {
        public IPactBuilder PactBuilder { get; private set; }
        public IMockProviderService MockProviderService { get; private set; }

        public int MockServerPort { get { return 9222; } }
        public string MockProviderServiceBaseUri { get { return String.Format("http://localhost:{0}", MockServerPort); } }

        public ConsumerOrderPact()
        {
            PactBuilder = new PactBuilder();

            PactBuilder
              .ServiceConsumer("Consumer")
              .HasPactWith("Something API");

            MockProviderService = PactBuilder.MockService(MockServerPort);
        }

        public void Dispose()
        {
            PactBuilder.Build();
        }
    }
}
