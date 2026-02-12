using Garage_Management.Base.Entities.Accounts;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace Garage_Management.Base.Data
{
    public static class InitialSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            var passwordHasher = new PasswordHasher<User>();

            modelBuilder.Entity<IdentityRole<int>>().HasData(
                new IdentityRole<int> { Id = 1, Name = "Customer", NormalizedName = "CUSTOMER" },
                new IdentityRole<int> { Id = 2, Name = "Receptionist", NormalizedName = "RECEPTIONIST" },
                new IdentityRole<int> { Id = 3, Name = "Supervisor", NormalizedName = "SUPERVISOR" },
                new IdentityRole<int> { Id = 4, Name = "Mechanic", NormalizedName = "MECHANIC" },
                new IdentityRole<int> { Id = 5, Name = "Stoker", NormalizedName = "STOKER" },
                new IdentityRole<int> { Id = 6, Name = "Admin", NormalizedName = "ADMIN" }
            );

            modelBuilder.Entity<IdentityUserRole<int>>().HasData(
                 new IdentityUserRole<int> { UserId = 1, RoleId = 6 }, // Admin
                 new IdentityUserRole<int> { UserId = 2, RoleId = 2 }, // Receptionist
                 new IdentityUserRole<int> { UserId = 3, RoleId = 3 }, // Supervisor
                 new IdentityUserRole<int> { UserId = 4, RoleId = 4 }, // Mechanic
                 new IdentityUserRole<int> { UserId = 5, RoleId = 5 },  // Stoker
                 new IdentityUserRole<int> { UserId = 10, RoleId = 1 },  // Customer
                 new IdentityUserRole<int> { UserId = 11, RoleId = 1 }  // Customer

             );

            modelBuilder.Entity<User>().HasData(
                // Admin & Staff
                new User
                {
                    Id = 1,
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    Email = "admin@garage.vn",
                    NormalizedEmail = "ADMIN@GARAGE.VN",
                    EmailConfirmed = true,
                    PasswordHash = passwordHasher.HashPassword(null, "Admin@123!"),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString("N"),
                    PhoneNumber = "0909123456",
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                },
                new User
                {
                    Id = 2,
                    UserName = "manager01",
                    NormalizedUserName = "MANAGER01",
                    Email = "manager01@garage.vn",
                    NormalizedEmail = "MANAGER01@GARAGE.VN",
                    EmailConfirmed = true,
                    PasswordHash = passwordHasher.HashPassword(null, "Manager@123!"),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString("N"),
                    PhoneNumber = "0912345678",
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                },
                new User
                {
                    Id = 3,
                    UserName = "mechanic01",
                    NormalizedUserName = "MECHANIC01",
                    Email = "mechanic01@garage.vn",
                    NormalizedEmail = "MECHANIC01@GARAGE.VN",
                    EmailConfirmed = true,
                    PasswordHash = passwordHasher.HashPassword(null, "Mech@123!"),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString("N"),
                    PhoneNumber = "0987654321",
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                },
                new User
                {
                    Id = 4,
                    UserName = "recep01",
                    NormalizedUserName = "RECEP01",
                    Email = "reception@garage.vn",
                    NormalizedEmail = "RECEPTION@GARAGE.VN",
                    EmailConfirmed = true,
                    PasswordHash = passwordHasher.HashPassword(null, "Recep@123!"),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString("N"),
                    PhoneNumber = "0978123456",
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                },
                new User
                {
                    Id = 5,
                    UserName = "stocker01",
                    NormalizedUserName = "STOCKER01",
                    Email = "stocker@garage.vn",
                    NormalizedEmail = "STOCKER@GARAGE.VN",
                    EmailConfirmed = true,
                    PasswordHash = passwordHasher.HashPassword(null, "Stocker@123!"),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString("N"),
                    PhoneNumber = "0978123356",
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                },

                // Customer-linked users
                new User
                {
                    Id = 10,
                    UserName = "khachhang01",
                    NormalizedUserName = "KHACHHANG01",
                    Email = "nguyen.van.a@gmail.com",
                    NormalizedEmail = "NGUYEN.VAN.A@GMAIL.COM",
                    EmailConfirmed = true,
                    PasswordHash = passwordHasher.HashPassword(null, "Khach@123!"),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString("N"),
                    PhoneNumber = "0912345670",
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                },
                new User
                {
                    Id = 11,
                    UserName = "khachhang02",
                    NormalizedUserName = "KHACHHANG02",
                    Email = "tran.thi.b@gmail.com",
                    NormalizedEmail = "TRAN.THI.B@GMAIL.COM",
                    EmailConfirmed = true,
                    PasswordHash = passwordHasher.HashPassword(null, "Khach@123!"),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString("N"),
                    PhoneNumber = "0987654312",
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                }
            );

            // 3. Seed Employees
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    EmployeeId = 1,
                    UserId = 1, // admin
                    FirstName = "Nguyễn Văn",
                    LastName = "Admin",
                    EmployeeCode = "NV-ADMIN-001",
                    Position = "Quản trị hệ thống",
                    IsActive = true,
                    StartWorkingDate = new DateTime(2024, 1, 1),
                    CreatedAt = DateTime.UtcNow
                },
                new Employee
                {
                    EmployeeId = 2,
                    UserId = 2, // manager
                    FirstName = "Trần Thị",
                    LastName = "Quản",
                    EmployeeCode = "QL-001",
                    Position = "Quản lý gara",
                    IsActive = true,
                    StartWorkingDate = new DateTime(2024, 2, 1),
                    CreatedAt = DateTime.UtcNow
                },
                new Employee
                {
                    EmployeeId = 3,
                    UserId = 3, // mechanic
                    FirstName = "Lê Văn",
                    LastName = "Hùng",
                    EmployeeCode = "KT-001",
                    Position = "Kỹ thuật viên",
                    IsActive = true,
                    StartWorkingDate = new DateTime(2024, 3, 10),
                    CreatedAt = DateTime.UtcNow
                },
                new Employee
                {
                    EmployeeId = 4,
                    UserId = 4, // receptionist
                    FirstName = "Phạm Thị",
                    LastName = "Lan",
                    EmployeeCode = "LT-001",
                    Position = "Lễ tân",
                    IsActive = true,
                    StartWorkingDate = new DateTime(2024, 4, 1),
                    CreatedAt = DateTime.UtcNow
                }
            );

            // 4. Seed Customers
            modelBuilder.Entity<Customer>().HasData(
                // Có tài khoản đăng nhập
                new Customer
                {
                    CustomerId = 1,
                    FirstName = "Nguyễn Văn",
                    LastName = "An",
                    Address = "123 Đường Láng, Đống Đa, Hà Nội",
                    UserId = 10,
                    CreatedAt = DateTime.UtcNow
                },
                new Customer
                {
                    CustomerId = 2,
                    FirstName = "Trần Thị",
                    LastName = "Bình",
                    Address = "45 Nguyễn Trãi, Thanh Xuân, Hà Nội",
                    UserId = 11,
                    CreatedAt = DateTime.UtcNow
                }

            );
        }
    }
}

