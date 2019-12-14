using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using OrderAPI.DataAccess.Contexts;
using OrderAPI.Models;

namespace OrderAPI.DataAccess.Repositories
{
    public class ProductRepository : Repository<Product>
    {
        readonly ProductContext _context;

        public ProductRepository(ProductContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public override async Task<Product> GetAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(product => product.ProductId == id);
        }

        public override async Task CreateAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public override async Task UpdateAsync(Product old, Product updated)
        {
            old.Name = updated.Name;
            old.Price = updated.Price;
            old.Stock = updated.Stock;
            await _context.SaveChangesAsync();
        }

        public override async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}