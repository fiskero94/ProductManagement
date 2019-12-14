using Microsoft.EntityFrameworkCore;
using OrderManager.DBContexts;
using OrderManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderManager.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductContext _dbContext;

        public ProductRepository(ProductContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Product> GetProductByIDAsync(int productId)
        {
            return await _dbContext.Products.FindAsync(productId);
        }

        public async Task InsertProductAsync(Product product)
        {
            _dbContext.Add(product);
            await SaveAsync();
        }

        public async Task UpdateProductAsync(Product oldProduct, Product updatedProduct)
        {
            oldProduct.Stock = updatedProduct.Stock;
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
        public static ProductRepository GetRepository()
        {
            DbContextOptionsBuilder<ProductContext> opt = new DbContextOptionsBuilder<ProductContext>();
            opt.UseSqlServer(ProductContext.ConnectionString);

            return new ProductRepository(new ProductContext(opt.Options));
        }
    }
}
