using Infrastructure.DataAccess;
using Infrastructure.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OrderAPI.Controllers;
using OrderAPI.Models;
using System;
using System.Threading.Tasks;

namespace OrderTests
{
    [TestClass]
    public class OrderControllerTest
    {
        [TestMethod]
        public async Task OrderController_GetAsync_Ok()
        {
            // Arrange
            Mock<IRepository<Product>> productRepositoryMock = new Mock<IRepository<Product>>();
            Mock<IRepository<Order>> orderRepositoryMock = new Mock<IRepository<Order>>();
            Mock<IRabbitMQMessenger> messengerMock = new Mock<IRabbitMQMessenger>();
            OrderController controller = new OrderController(orderRepositoryMock.Object, productRepositoryMock.Object, messengerMock.Object);
            DateTime date = DateTime.Parse("08/18/2018 07:22:16");
            int orderId = 1;
            int productId = 2;

            orderRepositoryMock.Setup(repo => repo.GetAsync(1)).Returns(Task.FromResult(new Order()
            {
                Date = date,
                OrderId = orderId,
                ProductId = productId
            }));

            // Act
            IActionResult result = await controller.GetAsync(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(200, (result as OkObjectResult).StatusCode);
            Order order = (Order)(result as OkObjectResult).Value;
            Assert.AreEqual(date, order.Date);
            Assert.AreEqual(orderId, order.OrderId);
            Assert.AreEqual(productId, order.ProductId);
        }

        [TestMethod]
        public async Task OrderController_GetAsync_NotFound()
        {
            // Arrange
            Mock<IRepository<Product>> productRepositoryMock = new Mock<IRepository<Product>>();
            Mock<IRepository<Order>> orderRepositoryMock = new Mock<IRepository<Order>>();
            Mock<IRabbitMQMessenger> messengerMock = new Mock<IRabbitMQMessenger>();
            OrderController controller = new OrderController(orderRepositoryMock.Object, productRepositoryMock.Object, messengerMock.Object);
            Order setup = null;

            orderRepositoryMock.Setup(repo => repo.GetAsync(1)).Returns(Task.FromResult(setup));

            // Act
            IActionResult result = await controller.GetAsync(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            Assert.AreEqual(404, (result as NotFoundObjectResult).StatusCode);
        }
    }
}