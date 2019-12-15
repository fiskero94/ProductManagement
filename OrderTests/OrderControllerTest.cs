using Infrastructure.DataAccess;
using Infrastructure.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OrderAPI.Controllers;
using OrderAPI.Models;
using OrderAPI.Models.ViewModels;
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
            DateTime date = DateTime.Parse("2018-08-18T07:22:16.0000000Z");
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

        [TestMethod]
        public async Task OrderController_PostAsync_Ok()
        {
            // Arrange
            Mock<IRepository<Product>> productRepositoryMock = new Mock<IRepository<Product>>();
            Mock<IRepository<Order>> orderRepositoryMock = new Mock<IRepository<Order>>();
            Mock<IRabbitMQMessenger> messengerMock = new Mock<IRabbitMQMessenger>();
            OrderController controller = new OrderController(orderRepositoryMock.Object, productRepositoryMock.Object, messengerMock.Object);

            int orderId = 1;
            int productId = 1;
            string name = "Product";
            decimal price = 100.00M;
            int stock = 10;

            productRepositoryMock.Setup(repo => repo.GetAsync(1)).Returns(Task.FromResult(new Product()
            {
                ProductId = productId,
                Name = name,
                Price = price,
                Stock = stock
            }));

            orderRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Order>())).Callback<Order>(o => o.OrderId = orderId);

            OrderViewModel viewModel = new OrderViewModel() { ProductId = productId };

            // Act
            IActionResult result = await controller.PostAsync(viewModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(200, (result as OkObjectResult).StatusCode);
            Order order = (Order)(result as OkObjectResult).Value;
            Assert.AreEqual(orderId, order.OrderId);
            Assert.AreEqual(productId, order.ProductId);
            messengerMock.Verify(messenger => messenger.PublishAsync("OrderCreated", It.IsAny<Order>()), Times.Once);
        }

        [TestMethod]
        public async Task OrderController_PostAsync_BadRequestViewModelNull()
        {

        }

        [TestMethod]
        public async Task OrderController_PostAsync_NotFound()
        {

        }

        [TestMethod]
        public async Task OrderController_PostAsync_BadRequestNotInStock()
        {

        }
    }
}