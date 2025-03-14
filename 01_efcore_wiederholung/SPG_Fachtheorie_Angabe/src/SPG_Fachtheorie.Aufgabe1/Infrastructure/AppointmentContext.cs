using Microsoft.EntityFrameworkCore;
using SPG_Fachtheorie.Aufgabe1.Model;

namespace SPG_Fachtheorie.Aufgabe1.Infrastructure
{
    public class AppointmentContext : DbContext
    {
        
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Cashier> Cashiers { get; set; }
        public DbSet<CashDesk> CashDesks { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentItem> PaymentItems { get; set; }
        
        public AppointmentContext(DbContextOptions options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasDiscriminator<string>("Type")
                .HasValue<Manager>("Manager")
                .HasValue<Cashier>("Cashier");
            
            modelBuilder.Entity<Employee>().OwnsOne(a => a.Address);
        }
    }
}