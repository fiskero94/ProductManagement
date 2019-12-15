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

        [TestMethod]
        public async Task ProductController_PostAsync_Ok()
        {
            //Arrange
            Mock<IRepository<Product>> productRepositoryMock = new Mock<IRepository<Product>>();
            Mock<IRabbitMQMessenger> messengerMock = new Mock<IRabbitMQMessenger>();
            ProductController controller = new ProductController(productRepositoryMock.Object, messengerMock.Object);

            string name = "SAMSUNG";
            decimal price = 69;
            int productId = 1;
            int stock = 5;

            Product product = new Product()
            {
                Name = name,
                Price = price,
                Stock = stock
            };

            productRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Product>())).Callback<Product>(p => p.ProductId = productId);

            // Act
            IActionResult result = await controller.PostAsync(product);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(200, (result as OkObjectResult).StatusCode);
            Product order = (Product)(result as OkObjectResult).Value;
            Assert.AreEqual(productId, product.ProductId);
            messengerMock.Verify(messenger => messenger.PublishAsync("ProductCreated", It.IsAny<Product>()), Times.Once);
        }

        [TestMethod]
        public async Task ProductController_DeleteAsync_NoContent()
        {
            //Arrange
            Mock<IRepository<Product>> productRepositoryMock = new Mock<IRepository<Product>>();
            Mock<IRabbitMQMessenger> messengerMock = new Mock<IRabbitMQMessenger>();
            ProductController controller = new ProductController(productRepositoryMock.Object, messengerMock.Object);

            int productId = 1;
            Product product = new Product()
            {
                ProductId = productId,
                Name = "Product",
                Price = 100.00M,
                Stock = 10,
            };

            productRepositoryMock.Setup(repo => repo.GetAsync(productId)).Returns(Task.FromResult(product));

            // Act
            IActionResult result = await controller.DeleteAsync(productId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            Assert.AreEqual(204, (result as NoContentResult).StatusCode);
            productRepositoryMock.Verify(mock => mock.DeleteAsync(product), Times.Once);
            messengerMock.Verify(mock => mock.PublishAsync("ProductDeleted", product), Times.Once);
        }

        [TestMethod]
        public async Task ProductController_DeleteAsync_NotFound()
        {
            //Arrange
            Mock<IRepository<Product>> productRepositoryMock = new Mock<IRepository<Product>>();
            Mock<IRabbitMQMessenger> messengerMock = new Mock<IRabbitMQMessenger>();
            ProductController controller = new ProductController(productRepositoryMock.Object, messengerMock.Object);

            int productId = 0;
            Product product = null;

            productRepositoryMock.Setup(repo => repo.GetAsync(productId)).Returns(Task.FromResult(product));

            // Act
            IActionResult result = await controller.DeleteAsync(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            Assert.AreEqual(404, (result as NotFoundObjectResult).StatusCode);
            Assert.AreEqual("The product could not be found.", (result as NotFoundObjectResult).Value);
        }



        [TestMethod]
        public async Task ProductController_PostAsync_BadRequest()
        {
            //Arrange
            Mock<IRepository<Product>> productRepositoryMock = new Mock<IRepository<Product>>();
            Mock<IRabbitMQMessenger> messengerMock = new Mock<IRabbitMQMessenger>();
            ProductController controller = new ProductController(productRepositoryMock.Object, messengerMock.Object);

            int productId = 1;
            Product product = null;

            productRepositoryMock.Setup(repo => repo.GetAsync(productId)).Returns(Task.FromResult(product));

            // Act
            IActionResult result = await controller.PostAsync(product);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(400, (result as BadRequestObjectResult).StatusCode);
            Assert.AreEqual("Product is null.", (result as BadRequestObjectResult).Value);
        }

    }
}




