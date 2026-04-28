using FacturacionAPI.DTOs;
using FacturacionAPI.Entities;
using FacturacionAPI.Repositories;

namespace FacturacionAPI.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllAsync();
        Task<ProductDTO?> GetByIdAsync(int id);
        Task<ProductDTO> CreateAsync(CreateProductDTO productDto);
        Task<ProductDTO?> UpdateAsync(int id, CreateProductDTO productDto);
        Task<bool> DeleteAsync(int id);
    }
}
