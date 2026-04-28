using FacturacionAPI.DTOs;
using FacturacionAPI.Entities;
using FacturacionAPI.Repositories;

namespace FacturacionAPI.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDTO>> GetAllAsync();
        Task<CustomerDTO?> GetByIdAsync(int id);
        Task<CustomerDTO> CreateAsync(CreateCustomerDTO customerDto);
        Task<CustomerDTO?> UpdateAsync(int id, CreateCustomerDTO customerDto);
        Task<bool> DeleteAsync(int id);
    }
}
