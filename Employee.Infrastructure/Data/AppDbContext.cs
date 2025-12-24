using Microsoft.EntityFrameworkCore;
using Employee.Domain.Entities;

namespace Employee.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee.Domain.Entities.Employee> Employees { get; set; }
        public DbSet<Client> Clients { get; set; }
    }
}
