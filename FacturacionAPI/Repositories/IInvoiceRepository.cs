using FacturacionAPI.Entities;

namespace FacturacionAPI.Repositories
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        Task<Invoice?> GetInvoiceWithDetailsAsync(int id);
        Task<string> GenerateInvoiceNumberAsync();
    }
}
