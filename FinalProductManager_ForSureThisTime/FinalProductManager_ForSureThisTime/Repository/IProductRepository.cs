using FinalProductManager_ForSureThisTime.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProductManager_ForSureThisTime.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProducts();
        Task<Product> GetProductByID(int ProductId);
        Task InsertProduct(Product Product);
        Task DeleteProduct(int ProductId);
        Task UpdateProduct(Product oldProduct, Product updatedProduct);
        Task Save();
    }
}
