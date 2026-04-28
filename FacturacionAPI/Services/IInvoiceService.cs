using FacturacionAPI.DTOs;

namespace FacturacionAPI.Services
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoiceResponseDTO>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<InvoiceDetailResponseDTO?> GetByIdAsync(int id);
        Task<InvoiceDetailResponseDTO> CreateAsync(CreateInvoiceDTO invoiceDto);
        Task<InvoiceDetailResponseDTO?> UpdateAsync(int id, CreateInvoiceDTO invoiceDto);
        Task<bool> CancelAsync(int id);
        Task<bool> MarkAsPaidAsync(int id);   // FIX: nuevo método
        Task<bool> DeleteAsync(int id);        // FIX: nuevo método
    }
}
