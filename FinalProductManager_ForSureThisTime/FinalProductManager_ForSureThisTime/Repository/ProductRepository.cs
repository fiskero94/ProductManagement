using FinalProductManager_ForSureThisTime.DBContexts;
using FinalProductManager_ForSureThisTime.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProductManager_ForSureThisTime.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductContext _dbContext;

        public ProductRepository(ProductContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task DeleteProduct(int productId)
        {
            var product = _dbContext.Products.Find(productId);
            _dbContext.Products.Remove(product);
            await Save();
        }

        public async Task<Product> GetProductByID(int productId)
        {
            return await _dbContext.Products.FindAsync(productId);
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _dbContext.Products.ToListAsync();
        }

        public async Task InsertProduct(Product product)
        {
            _dbContext.Add(product);
            await Save();
        }

        public async Task Save()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateProduct(Product oldProduct, Product updatedProduct)
        {
            oldProduct.Name = updatedProduct.Name;
            oldProduct.Price = updatedProduct.Price;
            oldProduct.Stock = updatedProduct.Stock;
            oldProduct.Description = updatedProduct.Description;
            oldProduct.CategoryId = updatedProduct.CategoryId;

            //_dbContext.Entry(product).State = EntityState.Modified;
            await Save();
        }

        public static ProductRepository GetRepository()
        {
            DbContextOptionsBuilder<ProductContext> maybe = new DbContextOptionsBuilder<ProductContext>();
            maybe.UseSqlServer(ProductContext.ConnectionString);

            return new ProductRepository(new ProductContext(maybe.Options));
        }
    }
}
