using Infrastructure.DataAccess;
using Infrastructure.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProductAPI.Controllers;
using ProductAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductTests
{
    [TestClass]
    public class ProductControllerTest
    {
        [TestMethod]
        public async Task ProductController_GetAllAsync_Ok()
        {

            // Arrange
            Mock<IRepository<Product>> productRepositoryMock = new Mock<IRepository<Product>>();
            Mock<IRabbitMQMessenger> messengerMock = new Mock<IRabbitMQMessenger>();
            ProductController controller = new ProductController(productRepositoryMock.Object, messengerMock.Object);
           
            var product1 = new Product() { Name = "Product", Price = 5, ProductId = 1, Stock = 5 };
            var product2 = new Product() { Name = "Product2", Price = 10, ProductId = 2, Stock = 10 };
            IEnumerable<Product> list = new List<Product>() { product1, product2 };
            
            productRepositoryMock.Setup(repo => repo.GetAllAsync()).Returns(Task.FromResult(list));

            // Act
            IActionResult result = await controller.GetAsync();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(200, (result as OkObjectResult).StatusCode);
            List<Product> product = (List<Product>)(result as OkObjectResult).Value;
            Assert.AreEqual(2, product.Count);
        }

        [TestMethod]
        public async Task ProductController_GetAsync_Ok()
        {
            //Arrange
            Mock<IRepository<Product>> productRepositoryMock = new Mock<IRepository<Product>>();
            Mock<IRabbitMQMessenger> messengerMock = new Mock<IRabbitMQMessenger>();
            ProductController controller = new ProductController(productRepositoryMock.Object, messengerMock.Object);

            string name = "IphoneX";
            decimal price = 69;
            int productId = 1;
            int stock = 5;

            productRepositoryMock.Setup(repo => repo.GetAsync(1)).Returns(Task.FromResult(new Product()
            {
                 Name = name,
                 Price = price,
                 ProductId = productId,
                 Stock = stock
            }));

            //Act
            IActionResult result = await controller.GetAsync(1);

            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(200, (result as OkObjectResult).StatusCode);
            Product product = (Product)(result as OkObjectResult).Value;
            Assert.AreEqual(name, product.Name);
            Assert.AreEqual(price, product.Price);
            Assert.AreEqual(productId, product.ProductId);
            Assert.AreEqual(stock, product.Stock);
        }

        [TestMethod]
        public async Task ProductController_GetAsync_NotFound()
        {
            //Arrange
            Mock<IRepository<Product>> productRepositoryMock = new Mock<IRepository<Product>>();
            Mock<IRabbitMQMessenger> messengerMock = new Mock<IRabbitMQMessenger>();
            ProductController controller = new ProductController(productRepositoryMock.Object, messengerMock.Object);

            Product setup = null;

            productRepositoryMock.Setup(repo => repo.GetAsync(1)).Returns(Task.FromResult(setup));

            // Act
            IActionResult result = await controller.GetAsync(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            Assert.AreEqual(404, (result as NotFoundObjectResult).StatusCode);
        }


    }
}
