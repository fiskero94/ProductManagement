using FinalProductManager_ForSureThisTime.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProductManager_ForSureThisTime.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIDAsync(int ProductId);
        Task InsertProductAsync(Product Product);
        Task DeleteProductAsync(int ProductId);
        Task UpdateProductAsync(Product oldProduct, Product updatedProduct);
        Task SaveAsync();
    }
}
