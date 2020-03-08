using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Linq;
using System.Web;

namespace RapidWeb.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<ServiceCity> ServiceCities { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<Complain> Complains { get; set; }
        public DbSet<ComplainProductsDetail> ComplainProductsDetails { get; set; }

        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public DbSet<PurchaseRequestProduct> PurchaseRequestProducts { get; set; }

        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderProduct> PurchaseOrderProducts { get; set; }

        public DbSet<StockIn> StockIns { get; set; }
        public DbSet<StockInProduct> StockInProducts { get; set; }

        public DbSet<StockOut> StockOuts { get; set; }
        public DbSet<StockOutProduct> StockOutProducts { get; set; }

        public DbSet<SalesTaxInvoice> SalesTaxInvoices { get; set; }
        public DbSet<SaleTexInvoiceProduct> SaleTexInvoiceProducts { get; set; }

        public DbSet<NormalInvoice> NormalInvoices { get; set; }
        public DbSet<NormalInvoiceProduct> NormalInvoiceProducts { get; set; }

        public DbSet<Payroll> Payrolls { get; set; }
        public DbSet<EmployeeSalary> EmployeeSalaries { get; set; }

        public DbSet<DailyExpense> DailyExpenses { get; set; }
    }
}