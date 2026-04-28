namespace FacturacionAPI.DTOs
{
    public class LoginDTO { public string Email { get; set; } = string.Empty; public string Password { get; set; } = string.Empty; }
    public class RegisterDTO { public string Username { get; set; } = string.Empty; public string Email { get; set; } = string.Empty; public string Password { get; set; } = string.Empty; }
    public class AuthResponseDTO { public string Token { get; set; } = string.Empty; public string Email { get; set; } = string.Empty; public string Role { get; set; } = string.Empty; }
    public class ProductDTO { public int Id { get; set; } public string Name { get; set; } = string.Empty; public string Code { get; set; } = string.Empty; public string Description { get; set; } = string.Empty; public decimal Price { get; set; } public int Stock { get; set; } public string Category { get; set; } = string.Empty; public string Unit { get; set; } = "unidad"; }
    public class CreateProductDTO { public string Name { get; set; } = string.Empty; public string Code { get; set; } = string.Empty; public string Description { get; set; } = string.Empty; public decimal Price { get; set; } public int Stock { get; set; } public string Category { get; set; } = string.Empty; public string Unit { get; set; } = "unidad"; }
    public class CustomerDTO { public int Id { get; set; } public string Name { get; set; } = string.Empty; public string Email { get; set; } = string.Empty; public string Phone { get; set; } = string.Empty; public string Address { get; set; } = string.Empty; public string DocumentNumber { get; set; } = string.Empty; }
    public class CreateCustomerDTO { public string Name { get; set; } = string.Empty; public string Email { get; set; } = string.Empty; public string Phone { get; set; } = string.Empty; public string Address { get; set; } = string.Empty; public string DocumentNumber { get; set; } = string.Empty; }
    public class CreateInvoiceDTO { public int CustomerId { get; set; } public DateTime? Date { get; set; } public DateTime? DueDate { get; set; } public List<InvoiceItemDTO> Items { get; set; } = new(); }
    public class InvoiceItemDTO { public int ProductId { get; set; } public int Quantity { get; set; } public decimal UnitPrice { get; set; } }
    public class InvoiceResponseDTO { public int Id { get; set; } public string InvoiceNumber { get; set; } = string.Empty; public int CustomerId { get; set; } public string CustomerName { get; set; } = string.Empty; public DateTime Date { get; set; } public DateTime? DueDate { get; set; } public decimal Subtotal { get; set; } public decimal Tax { get; set; } public decimal Total { get; set; } public string Status { get; set; } = string.Empty; }
    // FIX: InvoiceDetailResponseDTO hereda de InvoiceResponseDTO correctamente
    public class InvoiceDetailResponseDTO : InvoiceResponseDTO { public List<InvoiceDetailItemDTO> Items { get; set; } = new(); }
    // FIX: añadir ProductId al DTO de item para que el frontend pueda poblar el select al editar
    public class InvoiceDetailItemDTO { public int ProductId { get; set; } public string ProductName { get; set; } = string.Empty; public int Quantity { get; set; } public decimal UnitPrice { get; set; } public decimal Subtotal { get; set; } }
}
