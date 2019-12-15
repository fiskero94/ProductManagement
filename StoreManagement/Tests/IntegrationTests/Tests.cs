using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IntegrationTests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            JsonSerializerSettings serializerSettings;

            serializerSettings = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };

            serializerSettings.Converters.Add(new StringEnumConverter
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            });

            using (HttpClient client = new HttpClient())
            {
                // Create Product
                StringContent productCreate = new StringContent("{\"name\": \"Test\",\"price\": 100.00,\"stock\": 1}", Encoding.UTF8, "application/json");
                HttpResponseMessage productCreateResponse = await client.PostAsync("http://localhost:5102/api/v1/product", productCreate);
                Assert.AreEqual(HttpStatusCode.OK, productCreateResponse.StatusCode);
                JObject productCreateResponseObject = JsonConvert.DeserializeObject<JObject>(await productCreateResponse.Content.ReadAsStringAsync());
                int productId = productCreateResponseObject.GetValue("productId").ToObject<int>();

                Thread.Sleep(2000);

                // Create Order
                StringContent orderCreate = new StringContent("{\"productId\": " + productId + "}", Encoding.UTF8, "application/json");
                HttpResponseMessage orderCreateResponse = await client.PostAsync("http://localhost:5101/api/v1/order", orderCreate);
                Assert.AreEqual(HttpStatusCode.OK, orderCreateResponse.StatusCode);
                JObject orderCreateResponseObject = JsonConvert.DeserializeObject<JObject>(await orderCreateResponse.Content.ReadAsStringAsync());
                int orderId = orderCreateResponseObject.GetValue("orderId").ToObject<int>();

                Thread.Sleep(2000);

                // Create Order
                StringContent orderCreate2 = new StringContent("{\"productId\": " + productId + "}", Encoding.UTF8, "application/json");
                HttpResponseMessage orderCreateResponse2 = await client.PostAsync("http://localhost:5101/api/v1/order", orderCreate2);
                Assert.AreEqual(HttpStatusCode.BadRequest, orderCreateResponse2.StatusCode);
            }
        }
    }
}