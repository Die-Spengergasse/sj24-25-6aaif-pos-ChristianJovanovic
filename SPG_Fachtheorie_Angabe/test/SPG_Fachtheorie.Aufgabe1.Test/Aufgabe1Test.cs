using Microsoft.EntityFrameworkCore;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Model;
using System;
using System.Linq;
using Xunit;

namespace SPG_Fachtheorie.Aufgabe1.Test
{
    [Collection("Sequential")]
    public class Aufgabe1Test
    {
        private AppointmentContext GetEmptyDbContext()
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlite(@"Data Source=cash.db")
                .Options;

            var db = new AppointmentContext(options);
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            return db;
        }

        // Creates an empty DB in Debug\net8.0\cash.db
        [Fact]
        public void CreateDatabaseTest()
        {
            using var db = GetEmptyDbContext();
        }

        [Fact]
        public void AddCashierSuccessTest()
        {
            var db = GetEmptyDbContext();
            var address = new Address("Musterstraße", "Wien", "1220");
            var cashier = new Cashier(1, "Max", "Mustermann", address, "Kassa");
            db.Add(cashier);
            db.SaveChanges();
            Assert.True(db.Cashiers.Count() == 1);
        }

        [Fact]
        public void AddPaymentSuccessTest()
        {
            var db = GetEmptyDbContext();
            var cashDesk = new CashDesk(1);
            var address = new Address("Musterstraße", "Wien", "1220");
            var cashier = new Cashier(2, "Max", "Mustermann", address, "Kassa");
            var payment = new Payment(cashDesk, new DateTime(2011, 6, 10), PaymentType.Cash, cashier);
            
            db.Add(payment);
            db.SaveChanges();
            
            Assert.True(db.Payments.Count() == 1);
        }

        [Fact]
        public void EmployeeDiscriminatorSuccessTest()
        {
            var db = GetEmptyDbContext();
            var address = new Address("Musterstraße", "Wien", "1220");
            var cashier = new Cashier(3, "Max", "Mustermann", address, "Kassa");
            var manager = new Manager(4, "Max", "Mustermann", address, "Kassa");
            db.Add(cashier);
            db.Add(manager);
            db.SaveChanges();
            
        }
    }
}