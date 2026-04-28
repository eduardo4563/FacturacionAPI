using FacturacionAPI.DTOs;
using FacturacionAPI.Entities;
using FacturacionAPI.Repositories;

namespace FacturacionAPI.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IGenericRepository<Customer> _customerRepository;
        public CustomerService(IGenericRepository<Customer> customerRepository) { _customerRepository = customerRepository; }
        
        public async Task<IEnumerable<CustomerDTO>> GetAllAsync() => 
            (await _customerRepository.GetAllAsync()).Select(c => new CustomerDTO { Id = c.Id, Name = c.Name, Email = c.Email, Phone = c.Phone, DocumentNumber = c.DocumentNumber });
            
        public async Task<CustomerDTO?> GetByIdAsync(int id) { var c = await _customerRepository.GetByIdAsync(id); return c == null ? null : new CustomerDTO { Id = c.Id, Name = c.Name, Email = c.Email, Phone = c.Phone, DocumentNumber = c.DocumentNumber }; }
        
        public async Task<CustomerDTO> CreateAsync(CreateCustomerDTO dto) { var c = new Customer { Name = dto.Name, Email = dto.Email, Phone = dto.Phone, Address = dto.Address, DocumentNumber = dto.DocumentNumber }; var created = await _customerRepository.AddAsync(c); return new CustomerDTO { Id = created.Id, Name = created.Name, Email = created.Email, Phone = created.Phone, DocumentNumber = created.DocumentNumber }; }
        
        public async Task<CustomerDTO?> UpdateAsync(int id, CreateCustomerDTO dto) { var c = await _customerRepository.GetByIdAsync(id); if (c == null) return null; c.Name = dto.Name; c.Email = dto.Email; c.Phone = dto.Phone; c.Address = dto.Address; c.DocumentNumber = dto.DocumentNumber; await _customerRepository.UpdateAsync(c); return new CustomerDTO { Id = c.Id, Name = c.Name, Email = c.Email, Phone = c.Phone, DocumentNumber = c.DocumentNumber }; }
        
        public async Task<bool> DeleteAsync(int id) { var c = await _customerRepository.GetByIdAsync(id); if (c == null) return false; await _customerRepository.DeleteAsync(id); return true; }
    }
}
