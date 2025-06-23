using Microsoft.EntityFrameworkCore;

namespace SignalR.Models
{
    public class ShopifyEntity : DbContext
    {
        public ShopifyEntity()
        {
            
        }
        public ShopifyEntity(DbContextOptions options) : base(options) 
        {
            
        }

        public DbSet<Product> Products { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(LocalDb)\\MSSQLLocalDB;Database=YourDatabaseName;Trusted_Connection=True;");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
