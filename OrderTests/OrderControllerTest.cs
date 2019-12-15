using Infrastructure.DataAccess;
using Infrastructure.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OrderAPI.Controllers;
using OrderAPI.Models;
using OrderAPI.Models.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
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
        public async Task OrderController_GetAllAsync_Ok()
        {
            //return Ok(await _orderRepository.GetAllAsync());

            // Arrange
            Mock<IRepository<Product>> productRepositoryMock = new Mock<IRepository<Product>>();
            Mock<IRepository<Order>> orderRepositoryMock = new Mock<IRepository<Order>>();
            Mock<IRabbitMQMessenger> messengerMock = new Mock<IRabbitMQMessenger>();
            OrderController controller = new OrderController(orderRepositoryMock.Object, productRepositoryMock.Object, messengerMock.Object);

            DateTime orderDate1 = DateTime.Parse("2018-08-18T07:22:16.0000000Z");
            int orderId1 = 1;
            int productId1 = 2;
            DateTime orderDate2 = DateTime.Parse("2018-08-18T07:22:16.0000000Z");
            int orderId2 = 2;
            int productId2 = 3;

            Order order1 = new Order() { Date = orderDate1, OrderId = orderId1, ProductId = productId1 };
            Order order2 = new Order() { Date = orderDate2, OrderId = orderId2, ProductId = productId2 };
            IEnumerable<Order> orders = new List<Order>() { order1, order2};
            orderRepositoryMock.Setup(repo => repo.GetAllAsync()).Returns(Task.FromResult(orders));

            // Act
            IActionResult result = await controller.GetAsync();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(200, (result as OkObjectResult).StatusCode);

            List<Order> actualOrders = (List<Order>)(result as OkObjectResult).Value;
            Assert.AreEqual(orderDate1, actualOrders[0].Date);
            Assert.AreEqual(orderId1, actualOrders[0].OrderId);
            Assert.AreEqual(productId1, actualOrders[0].ProductId);
            Assert.AreEqual(2, actualOrders.Count);
        }

        [TestMethod]
        public async Task OrderController_DeleteAsync_NoContent()
        {
            // Arrange
            Mock<IRepository<Product>> productRepositoryMock = new Mock<IRepository<Product>>();
            Mock<IRepository<Order>> orderRepositoryMock = new Mock<IRepository<Order>>();
            Mock<IRabbitMQMessenger> messengerMock = new Mock<IRabbitMQMessenger>();
            OrderController controller = new OrderController(orderRepositoryMock.Object, productRepositoryMock.Object, messengerMock.Object);
            DateTime date = DateTime.Parse("2018-08-18T07:22:16.0000000Z");
            int orderId = 1;
            int productId = 2;

            Order order = new Order();
            order.Date = date;
            order.OrderId = orderId;
            order.ProductId = productId;

            orderRepositoryMock.Setup(repo => repo.GetAsync(1)).Returns(Task.FromResult(order));
            orderRepositoryMock.Setup(repo => repo.DeleteAsync(order)).Returns(Task.FromResult(true));

            // Act
            IActionResult result = await controller.DeleteAsync(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            Assert.AreEqual(204, (result as NoContentResult).StatusCode);
            orderRepositoryMock.Verify(mock => mock.DeleteAsync(order), Times.Once);
        }

        [TestMethod]
        public async Task OrderController_DeleteAsync_NotFound()
        {
            // Arrange
            Mock<IRepository<Product>> productRepositoryMock = new Mock<IRepository<Product>>();
            Mock<IRepository<Order>> orderRepositoryMock = new Mock<IRepository<Order>>();
            Mock<IRabbitMQMessenger> messengerMock = new Mock<IRabbitMQMessenger>();
            OrderController controller = new OrderController(orderRepositoryMock.Object, productRepositoryMock.Object, messengerMock.Object);
            Order order = new Order();
            order = null;
            orderRepositoryMock.Setup(repo => repo.GetAsync(1)).Returns(Task.FromResult(order));

            // Act
            IActionResult result = await controller.DeleteAsync(1);

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
            // Arrange
            Mock<IRepository<Product>> productRepositoryMock = new Mock<IRepository<Product>>();
            Mock<IRepository<Order>> orderRepositoryMock = new Mock<IRepository<Order>>();
            Mock<IRabbitMQMessenger> messengerMock = new Mock<IRabbitMQMessenger>();
            OrderController controller = new OrderController(orderRepositoryMock.Object, productRepositoryMock.Object, messengerMock.Object);
            OrderViewModel viewModel = null;

            // Act
            IActionResult result = await controller.PostAsync(viewModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(400, (result as BadRequestObjectResult).StatusCode);
            Assert.AreEqual("Order is null.", (result as BadRequestObjectResult).Value);
        }

        [TestMethod]
        public async Task OrderController_PostAsync_NotFound()
        {
            // Arrange
            Mock<IRepository<Product>> productRepositoryMock = new Mock<IRepository<Product>>();
            Mock<IRepository<Order>> orderRepositoryMock = new Mock<IRepository<Order>>();
            Mock<IRabbitMQMessenger> messengerMock = new Mock<IRabbitMQMessenger>();
            OrderController controller = new OrderController(orderRepositoryMock.Object, productRepositoryMock.Object, messengerMock.Object);
            
            int productId = 1;
            Product product = null;
            OrderViewModel viewModel = new OrderViewModel() { ProductId = productId };
            productRepositoryMock.Setup(repo => repo.GetAsync(productId)).Returns(Task.FromResult(product));

            // Act
            IActionResult result = await controller.PostAsync(viewModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            Assert.AreEqual(404, (result as NotFoundObjectResult).StatusCode);
            Assert.AreEqual("The product could not be found.", (result as NotFoundObjectResult).Value);
        }

        [TestMethod]
        public async Task OrderController_PostAsync_BadRequestNotInStock()
        {
            // Arrange
            Mock<IRepository<Product>> productRepositoryMock = new Mock<IRepository<Product>>();
            Mock<IRepository<Order>> orderRepositoryMock = new Mock<IRepository<Order>>();
            Mock<IRabbitMQMessenger> messengerMock = new Mock<IRabbitMQMessenger>();
            OrderController controller = new OrderController(orderRepositoryMock.Object, productRepositoryMock.Object, messengerMock.Object);

            int productId = 1;
            string name = "Product";
            decimal price = 100.00M;
            int stock = 0;

            productRepositoryMock.Setup(repo => repo.GetAsync(1)).Returns(Task.FromResult(new Product()
            {
                ProductId = productId,
                Name = name,
                Price = price,
                Stock = stock
            }));

            OrderViewModel viewModel = new OrderViewModel() { ProductId = productId };

            // Act
            IActionResult result = await controller.PostAsync(viewModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(400, (result as BadRequestObjectResult).StatusCode);
            Assert.AreEqual("The product is not in stock.", (result as BadRequestObjectResult).Value);
        }
    }
}