using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FacturacionAPI.DTOs;
using FacturacionAPI.Services;

namespace FacturacionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]   // FIX: re-habilitado (estaba comentado)
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomersController(ICustomerService customerService) { _customerService = customerService; }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _customerService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var c = await _customerService.GetByIdAsync(id);
            return c == null ? NotFound() : Ok(c);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerDTO dto) => Ok(await _customerService.CreateAsync(dto));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateCustomerDTO dto)
        {
            var c = await _customerService.UpdateAsync(id, dto);
            return c == null ? NotFound() : Ok(c);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _customerService.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
