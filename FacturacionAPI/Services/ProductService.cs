using FacturacionAPI.DTOs;
using FacturacionAPI.Entities;
using FacturacionAPI.Repositories;

namespace FacturacionAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _productRepository;
        public ProductService(IGenericRepository<Product> productRepository) { _productRepository = productRepository; }
        
        public async Task<IEnumerable<ProductDTO>> GetAllAsync() => 
            (await _productRepository.GetAllAsync()).Select(p => new ProductDTO { Id = p.Id, Name = p.Name, Code = p.Code, Description = p.Description, Price = p.Price, Stock = p.Stock, Category = p.Category, Unit = p.Unit });
            
        public async Task<ProductDTO?> GetByIdAsync(int id) { var p = await _productRepository.GetByIdAsync(id); return p == null ? null : new ProductDTO { Id = p.Id, Name = p.Name, Code = p.Code, Description = p.Description, Price = p.Price, Stock = p.Stock, Category = p.Category, Unit = p.Unit }; }
        
        public async Task<ProductDTO> CreateAsync(CreateProductDTO dto) { var p = new Product { Name = dto.Name, Code = dto.Code, Description = dto.Description, Price = dto.Price, Stock = dto.Stock, Category = dto.Category, Unit = dto.Unit }; var created = await _productRepository.AddAsync(p); return new ProductDTO { Id = created.Id, Name = created.Name, Code = created.Code, Description = created.Description, Price = created.Price, Stock = created.Stock, Category = created.Category, Unit = created.Unit }; }
        
        public async Task<ProductDTO?> UpdateAsync(int id, CreateProductDTO dto) { var p = await _productRepository.GetByIdAsync(id); if (p == null) return null; p.Name = dto.Name; p.Code = dto.Code; p.Description = dto.Description; p.Price = dto.Price; p.Stock = dto.Stock; p.Category = dto.Category; p.Unit = dto.Unit; await _productRepository.UpdateAsync(p); return new ProductDTO { Id = p.Id, Name = p.Name, Code = p.Code, Description = p.Description, Price = p.Price, Stock = p.Stock, Category = p.Category, Unit = p.Unit }; }
        
        public async Task<bool> DeleteAsync(int id) { var p = await _productRepository.GetByIdAsync(id); if (p == null) return false; await _productRepository.DeleteAsync(id); return true; }
    }
}
