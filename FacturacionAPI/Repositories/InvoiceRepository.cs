using Microsoft.EntityFrameworkCore;
using FacturacionAPI.Data;
using FacturacionAPI.Entities;

namespace FacturacionAPI.Repositories
{
    public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(ApplicationDbContext context) : base(context) { }
        
        public async Task<Invoice?> GetInvoiceWithDetailsAsync(int id) => 
            await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Details)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(i => i.Id == id);
                
        public async Task<string> GenerateInvoiceNumberAsync()
        {
            var lastInvoice = await _context.Invoices.OrderByDescending(i => i.Id).FirstOrDefaultAsync();
            int nextNumber = 1;
            if (lastInvoice?.InvoiceNumber != null)
            {
                var parts = lastInvoice.InvoiceNumber.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[1], out int lastNumber)) 
                    nextNumber = lastNumber + 1;
            }
            return $"F{DateTime.Now:yyyyMMdd}-{nextNumber:D4}";
        }
    }
}
