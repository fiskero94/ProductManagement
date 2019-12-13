using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductAPI.Models;

namespace ProductAPI.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductContext _dbContext;

        public ProductRepository(ProductContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string Decrypt(string projectId, string locationId, string keyRingId, string cryptoKeyId, byte[] ciphertextFile)
        {
            throw new NotImplementedException();
        }

        public byte[] Encrypt(string projectId, string locationId, string keyRingId, string cryptoKeyId, string plainTextFile)
        {
            throw new NotImplementedException();
        }

        public void EncryptData(int gin, byte[] result)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Product> GetProducts()
        {
            throw new NotImplementedException();
        }

        public Task<IList<Product>> GetProductsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
