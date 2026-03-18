using Garage_Management.Base.Entities;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Base.Entities.RepairEstimaties;
using Garage_Management.Base.Entities.Services;
using Garage_Management.Base.Entities.Vehiclies;
using Garage_Management.Base.Entities.Warranties;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Base.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSet - Accounts
       // public DbSet<User> Users => Set<User>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<AppointmentService> AppointmentServices => Set<AppointmentService>();
        public DbSet<AppointmentSparePart> AppointmentSpareParts => Set<AppointmentSparePart>();
       
        // DbSet - Vehicles
        public DbSet<VehicleBrand> VehicleBrands => Set<VehicleBrand>();
        public DbSet<VehicleModel> VehicleModels => Set<VehicleModel>();
        public DbSet<VehicleType> VehicleTypes => Set<VehicleType>();
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
      
        // DbSet - JobCards
        public DbSet<JobCard> JobCards => Set<JobCard>();
        public DbSet<JobCardMechanic> JobCardMechanics => Set<JobCardMechanic>();
        public DbSet<JobCardService> JobCardServices => Set<JobCardService>();
        public DbSet<JobCardSparePart> JobCardSpareParts => Set<JobCardSparePart>();
        public DbSet<JobCardLog> JobCardLogs => Set<JobCardLog>();
        public DbSet<JobCardServiceTask> JobCardServiceTasks => Set<JobCardServiceTask>();

        // DbSet - Inventories
        public DbSet<SparePartBrand> SparePartBrands => Set<SparePartBrand>();
        public DbSet<SparePartCategory> SparePartCategories => Set<SparePartCategory>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<Inventory> Inventories => Set<Inventory>();
        public DbSet<InventoryVehicleModel> InventoryVehicleModels => Set<InventoryVehicleModel>();
        public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();

        // DbSet -  Estimates
        public DbSet<RepairEstimate> RepairEstimates => Set<RepairEstimate>();
        public DbSet<RepairEstimateService> RepairEstimateServices => Set<RepairEstimateService>();
        public DbSet<RepairEstimateSparePart> RepairEstimateSpareParts => Set<RepairEstimateSparePart>();
        
        // DbSet - Invoice & Notifications
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<Notification> Notifications => Set<Notification>();

        // DbSet - Warranties
        public DbSet<SparePartWarrantyPolicy> SparePartWarrantyPolicies => Set<SparePartWarrantyPolicy>();
        public DbSet<ServiceWarrantyPolicy> ServiceWarrantyPolicies => Set<ServiceWarrantyPolicy>();
        public DbSet<WarrantyService> WarrantyServices => Set<WarrantyService>();
        public DbSet<WarrantySparePart> WarrantySpareParts => Set<WarrantySparePart>();

        // DbSet - Services 
        public DbSet<Service> Services => Set<Service>();
        public DbSet<ServiceVehicleType> ServiceVehicleTypes => Set<ServiceVehicleType>();
        public DbSet<ServiceTask> ServiceTasks => Set<ServiceTask>();
        public DbSet<WorkBay> WorkBay { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            InitialSeed.Seed(modelBuilder);
        }
    }
}
