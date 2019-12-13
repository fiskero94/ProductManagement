using ProductAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI.Repositories
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetProducts();
        Task<IList<Product>> GetProductsAsync();

        byte[] Encrypt(string projectId, string locationId, string keyRingId, string cryptoKeyId, string plainTextFile);

        string Decrypt(string projectId, string locationId, string keyRingId, string cryptoKeyId, byte[] ciphertextFile);

        void EncryptData(int gin, byte[] result);
    }
}
