using PactNet.Mocks.MockHttpService;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using PactNet.Mocks.MockHttpService.Models;

namespace OrderControllerUnitTests.Pacts
{
    public class ConsumerOrderTests : IClassFixture<ConsumerOrderPact>
    {
        private IMockProviderService mockProviderService;
        private string mockProviderServiceBaseUri;

        public ConsumerOrderTests(ConsumerOrderPact data)
        {
            mockProviderService = data.MockProviderService;
            mockProviderService.ClearInteractions();
            mockProviderServiceBaseUri = data.MockProviderServiceBaseUri;
        }

        [Fact]
        public void ProductCreated_Ok()
        {
            mockProviderService
                .Given("Product was created")
                .UponReceiving("Post request for product")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Post,
                    Path = ""
                });
        }
    }
}
