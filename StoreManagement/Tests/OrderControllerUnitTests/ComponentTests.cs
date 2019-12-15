using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OrderAPI.Controllers;
using OrderAPI.Models;
using OrderAPI.Models.ViewModels;

namespace OrderControllerUnitTests
{
    [TestClass]
    public class ComponentTests
    {

        OrderController controller;


        [TestMethod]
        public async Task OrderController_PostAsync_Ok()
        {
            // Arrange
            int orderId = 1;
            int productId = 1;

            OrderViewModel viewModel = new OrderViewModel() { ProductId = productId };

            // Act
            IActionResult result = await controller.PostAsync(viewModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(200, (result as OkObjectResult).StatusCode);
            Order order = (Order)(result as OkObjectResult).Value;
            Assert.AreEqual(orderId, order.OrderId);
            Assert.AreEqual(productId, order.ProductId);
        }
    }
}
