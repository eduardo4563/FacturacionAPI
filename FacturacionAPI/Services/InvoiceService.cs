using FacturacionAPI.DTOs;
using FacturacionAPI.Entities;
using FacturacionAPI.Repositories;

namespace FacturacionAPI.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IGenericRepository<Customer> _customerRepository;
        private readonly IGenericRepository<Product> _productRepository;

        public InvoiceService(IInvoiceRepository invoiceRepository, IGenericRepository<Customer> customerRepository, IGenericRepository<Product> productRepository)
        {
            _invoiceRepository = invoiceRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<InvoiceResponseDTO>> GetAllAsync(int page = 1, int pageSize = 10) =>
            (await _invoiceRepository.GetAllAsync())
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new InvoiceResponseDTO
                {
                    Id = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    CustomerId = i.CustomerId,
                    CustomerName = i.Customer?.Name ?? "N/A",
                    Date = i.Date,
                    DueDate = i.DueDate,
                    Total = i.Total,
                    Status = i.Status
                });

        public async Task<InvoiceDetailResponseDTO?> GetByIdAsync(int id)
        {
            var i = await _invoiceRepository.GetInvoiceWithDetailsAsync(id);
            if (i == null) return null;
            return new InvoiceDetailResponseDTO
            {
                Id = i.Id,
                InvoiceNumber = i.InvoiceNumber,
                CustomerId = i.CustomerId,
                CustomerName = i.Customer?.Name ?? "N/A",
                Date = i.Date,
                DueDate = i.DueDate,
                Total = i.Total,
                Status = i.Status,
                Items = i.Details.Select(d => new InvoiceDetailItemDTO
                {
                    ProductId = d.ProductId,
                    ProductName = d.Product?.Name ?? "N/A",
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Subtotal = d.Subtotal
                }).ToList()
            };
        }

        public async Task<InvoiceDetailResponseDTO> CreateAsync(CreateInvoiceDTO dto)
        {
            var customer = await _customerRepository.GetByIdAsync(dto.CustomerId)
                ?? throw new Exception("Cliente no encontrado");

            decimal subtotal = 0;
            var details = new List<InvoiceDetail>();
            // FIX: guardar nombres de producto durante el loop para la respuesta
            var productNames = new Dictionary<int, string>();

            foreach (var item in dto.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId)
                    ?? throw new Exception($"Producto {item.ProductId} no encontrado");
                if (product.Stock < item.Quantity)
                    throw new Exception($"Stock insuficiente para '{product.Name}' (disponible: {product.Stock})");

                decimal unitPrice = item.UnitPrice > 0 ? item.UnitPrice : product.Price;
                var itemSubtotal = unitPrice * item.Quantity;
                subtotal += itemSubtotal;
                productNames[item.ProductId] = product.Name;

                details.Add(new InvoiceDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice,
                    Subtotal = itemSubtotal
                });

                product.Stock -= item.Quantity;
                await _productRepository.UpdateAsync(product);
            }

            var tax = subtotal * 0.18m;
            var total = subtotal + tax;

            var invoice = new Invoice
            {
                InvoiceNumber = await _invoiceRepository.GenerateInvoiceNumberAsync(),
                CustomerId = dto.CustomerId,
                Date = dto.Date ?? DateTime.UtcNow,
                DueDate = dto.DueDate,
                Subtotal = subtotal,
                Tax = tax,
                Total = total,
                Status = "pendiente",   // FIX: crear como pendiente (se marca pagada manualmente)
                Details = details
            };

            var created = await _invoiceRepository.AddAsync(invoice);

            return new InvoiceDetailResponseDTO
            {
                Id = created.Id,
                InvoiceNumber = created.InvoiceNumber,
                CustomerId = created.CustomerId,
                CustomerName = customer.Name,
                Date = created.Date,
                DueDate = created.DueDate,
                Total = created.Total,
                Status = created.Status,
                // FIX: usar nombres guardados en el diccionario
                Items = details.Select(d => new InvoiceDetailItemDTO
                {
                    ProductId = d.ProductId,
                    ProductName = productNames.GetValueOrDefault(d.ProductId, "N/A"),
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Subtotal = d.Subtotal
                }).ToList()
            };
        }

        public async Task<InvoiceDetailResponseDTO?> UpdateAsync(int id, CreateInvoiceDTO dto)
        {
            var existingInvoice = await _invoiceRepository.GetInvoiceWithDetailsAsync(id);
            if (existingInvoice == null) return null;

            var customer = await _customerRepository.GetByIdAsync(dto.CustomerId)
                ?? throw new Exception("Cliente no encontrado");

            // Restaurar stock de items anteriores
            foreach (var detail in existingInvoice.Details)
            {
                var product = await _productRepository.GetByIdAsync(detail.ProductId);
                if (product != null)
                {
                    product.Stock += detail.Quantity;
                    await _productRepository.UpdateAsync(product);
                }
            }

            existingInvoice.Details.Clear();

            decimal subtotal = 0;
            var productNames = new Dictionary<int, string>();

            foreach (var item in dto.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId)
                    ?? throw new Exception($"Producto {item.ProductId} no encontrado");
                if (product.Stock < item.Quantity)
                    throw new Exception($"Stock insuficiente para '{product.Name}'");

                decimal unitPrice = item.UnitPrice > 0 ? item.UnitPrice : product.Price;
                var itemSubtotal = unitPrice * item.Quantity;
                subtotal += itemSubtotal;
                productNames[item.ProductId] = product.Name;

                existingInvoice.Details.Add(new InvoiceDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice,
                    Subtotal = itemSubtotal
                });

                product.Stock -= item.Quantity;
                await _productRepository.UpdateAsync(product);
            }

            var tax = subtotal * 0.18m;
            var total = subtotal + tax;

            existingInvoice.CustomerId = dto.CustomerId;
            existingInvoice.Date = dto.Date ?? existingInvoice.Date;
            existingInvoice.DueDate = dto.DueDate;
            existingInvoice.Subtotal = subtotal;
            existingInvoice.Tax = tax;
            existingInvoice.Total = total;

            await _invoiceRepository.UpdateAsync(existingInvoice);

            return new InvoiceDetailResponseDTO
            {
                Id = existingInvoice.Id,
                InvoiceNumber = existingInvoice.InvoiceNumber,
                CustomerId = existingInvoice.CustomerId,
                CustomerName = customer.Name,
                Date = existingInvoice.Date,
                DueDate = existingInvoice.DueDate,
                Total = existingInvoice.Total,
                Status = existingInvoice.Status,
                Items = existingInvoice.Details.Select(d => new InvoiceDetailItemDTO
                {
                    ProductId = d.ProductId,
                    ProductName = productNames.GetValueOrDefault(d.ProductId, "N/A"),
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Subtotal = d.Subtotal
                }).ToList()
            };
        }

        // FIX: status consistente en minúsculas "anulada" (antes decía "Anulado")
        public async Task<bool> CancelAsync(int id)
        {
            var invoice = await _invoiceRepository.GetInvoiceWithDetailsAsync(id);
            if (invoice == null || invoice.Status == "anulada") return false;

            // Restaurar stock al anular
            foreach (var detail in invoice.Details)
            {
                var product = await _productRepository.GetByIdAsync(detail.ProductId);
                if (product != null)
                {
                    product.Stock += detail.Quantity;
                    await _productRepository.UpdateAsync(product);
                }
            }

            invoice.Status = "anulada";
            await _invoiceRepository.UpdateAsync(invoice);
            return true;
        }

        // FIX: nuevo método para marcar como pagada
        public async Task<bool> MarkAsPaidAsync(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null || invoice.Status == "pagada" || invoice.Status == "anulada") return false;

            invoice.Status = "pagada";
            await _invoiceRepository.UpdateAsync(invoice);
            return true;
        }

        // FIX: nuevo método para eliminar factura
        public async Task<bool> DeleteAsync(int id)
        {
            var invoice = await _invoiceRepository.GetInvoiceWithDetailsAsync(id);
            if (invoice == null) return false;

            // Restaurar stock si la factura no estaba anulada
            if (invoice.Status != "anulada")
            {
                foreach (var detail in invoice.Details)
                {
                    var product = await _productRepository.GetByIdAsync(detail.ProductId);
                    if (product != null)
                    {
                        product.Stock += detail.Quantity;
                        await _productRepository.UpdateAsync(product);
                    }
                }
            }

            await _invoiceRepository.DeleteAsync(id);
            return true;
        }
    }
}
