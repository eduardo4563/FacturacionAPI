using Microsoft.EntityFrameworkCore;
using FacturacionAPI.Entities;

namespace FacturacionAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
            modelBuilder.Entity<Invoice>().Property(i => i.Subtotal).HasPrecision(18, 2);
            modelBuilder.Entity<Invoice>().Property(i => i.Tax).HasPrecision(18, 2);
            modelBuilder.Entity<Invoice>().Property(i => i.Total).HasPrecision(18, 2);
            modelBuilder.Entity<InvoiceDetail>().Property(d => d.UnitPrice).HasPrecision(18, 2);
            modelBuilder.Entity<InvoiceDetail>().Property(d => d.Subtotal).HasPrecision(18, 2);

            // Usuario admin por defecto
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@facturacion.com",
                PasswordHash = HashPassword("Admin123!"),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            });
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
