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

        public async Task DeleteProductAsync(int productId)
        {
            var product = _dbContext.Products.Find(productId);
            _dbContext.Products.Remove(product);
            await SaveAsync();
        }

        public async Task<Product> GetProductByIDAsync(int productId)
        {
            return await _dbContext.Products.FindAsync(productId);
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _dbContext.Products.ToListAsync();
        }

        public async Task InsertProductAsync(Product product)
        {
            _dbContext.Add(product);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product oldProduct, Product updatedProduct)
        {
            oldProduct.Name = updatedProduct.Name;
            oldProduct.Price = updatedProduct.Price;
            oldProduct.Stock = updatedProduct.Stock;
            oldProduct.Description = updatedProduct.Description;
            oldProduct.CategoryId = updatedProduct.CategoryId;

            //_dbContext.Entry(product).State = EntityState.Modified;
            await SaveAsync();
        }

        public static ProductRepository GetRepository()
        {
            DbContextOptionsBuilder<ProductContext> options = new DbContextOptionsBuilder<ProductContext>();
            options.UseSqlServer(ProductContext.ConnectionString);

            return new ProductRepository(new ProductContext(options.Options));
        }
    }
}
