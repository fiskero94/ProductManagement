using Infrastructure.DataAccess;
using Infrastructure.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OrderAPI.Controllers;
using OrderAPI.Models;
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

        [TestMethod]
        public async Task OrderController_GetAllAsync_Ok()
        {
            //return Ok(await _orderRepository.GetAllAsync());

            // Arrange
            Mock<IRepository<Product>> productRepositoryMock = new Mock<IRepository<Product>>();
            Mock<IRepository<Order>> orderRepositoryMock = new Mock<IRepository<Order>>();
            Mock<IRabbitMQMessenger> messengerMock = new Mock<IRabbitMQMessenger>();
            OrderController controller = new OrderController(orderRepositoryMock.Object, productRepositoryMock.Object, messengerMock.Object);

            DateTime orderDate1 = DateTime.Parse("08/18/2018 07:22:16");
            int orderId1 = 1;
            int productId1 = 2;
            DateTime orderDate2 = DateTime.Parse("08/18/2019 07:22:16");
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


    }
}