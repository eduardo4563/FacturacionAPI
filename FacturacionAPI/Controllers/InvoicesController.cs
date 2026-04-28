using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FacturacionAPI.DTOs;
using FacturacionAPI.Services;

namespace FacturacionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        public InvoicesController(IInvoiceService invoiceService) { _invoiceService = invoiceService; }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            // FIX: materializar a lista primero para evitar doble enumeración
            var invoices = (await _invoiceService.GetAllAsync(page, pageSize)).ToList();
            return Ok(new { page, pageSize, total = invoices.Count, data = invoices });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var i = await _invoiceService.GetByIdAsync(id);
            return i == null ? NotFound() : Ok(i);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInvoiceDTO dto)
        {
            try
            {
                if (dto == null) return BadRequest(new { message = "El payload no puede estar vacío" });
                if (dto.CustomerId <= 0) return BadRequest(new { message = "CustomerId debe ser mayor a 0" });
                if (dto.Items == null || !dto.Items.Any()) return BadRequest(new { message = "Debe incluir al menos un item" });

                var i = await _invoiceService.CreateAsync(dto);
                return Ok(i);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateInvoiceDTO dto)
        {
            try
            {
                if (dto.Items == null || !dto.Items.Any()) return BadRequest(new { message = "Debe incluir al menos un item" });
                var i = await _invoiceService.UpdateAsync(id, dto);
                return i == null ? NotFound() : Ok(i);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // FIX: endpoint para anular factura (antes solo existía /cancel)
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _invoiceService.CancelAsync(id);
            return result ? Ok(new { message = "Factura anulada" }) : NotFound(new { message = "Factura no encontrada o ya anulada" });
        }

        // FIX: nuevo endpoint para marcar como pagada
        [HttpPut("{id}/pay")]
        public async Task<IActionResult> MarkAsPaid(int id)
        {
            var result = await _invoiceService.MarkAsPaidAsync(id);
            return result ? Ok(new { message = "Factura marcada como pagada" }) : NotFound(new { message = "Factura no encontrada o no está pendiente" });
        }

        // FIX: nuevo endpoint DELETE para eliminar facturas
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _invoiceService.DeleteAsync(id);
            return result ? NoContent() : NotFound(new { message = "Factura no encontrada" });
        }
    }
}
