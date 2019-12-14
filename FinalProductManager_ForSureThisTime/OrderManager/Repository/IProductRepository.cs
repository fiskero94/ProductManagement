using OrderManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderManager.Repository
{
    public interface IProductRepository
    {
        Task<Product> GetProductByIDAsync(int productId);
        Task InsertProductAsync(Product product);
        Task UpdateProductAsync(Product oldProduct, Product updatedProduct);
        Task SaveAsync();
    }
}
