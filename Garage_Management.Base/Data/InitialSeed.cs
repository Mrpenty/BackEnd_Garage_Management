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
                new IdentityRole<int> { Id = 5, Name = "Stocker", NormalizedName = "STOCKER" },
                new IdentityRole<int> { Id = 6, Name = "Admin", NormalizedName = "ADMIN" }
            );

            modelBuilder.Entity<IdentityUserRole<int>>().HasData(
                 new IdentityUserRole<int> { UserId = 1, RoleId = 6 }, // Admin
                 new IdentityUserRole<int> { UserId = 2, RoleId = 3 }, // Supervisor
                 new IdentityUserRole<int> { UserId = 3, RoleId = 4 }, // Mechanic
                 new IdentityUserRole<int> { UserId = 4, RoleId = 2 }, // Receptionist
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
                    SecurityStamp = "5b6a4224-84dd-4305-b979-15b78c58c314",
                    ConcurrencyStamp = "38a8d0183ed74ac8a1c1249b08c413a3",
                    PhoneNumber = "0909123456",
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 3, 25, 7, 28, 5, 414, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 2,
                    UserName = "Supervisor01",
                    NormalizedUserName = "SUPERVISOR01",
                    Email = "manager01@garage.vn",
                    NormalizedEmail = "MANAGER01@GARAGE.VN",
                    EmailConfirmed = true,
                    PasswordHash = passwordHasher.HashPassword(null, "Manager@123!"),
                    SecurityStamp = "c61a1880-b13b-4590-9482-753068f03897",
                    ConcurrencyStamp = "5917370b62804f18af83f15b6380f997",
                    PhoneNumber = "0912345678",
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 3, 25, 7, 28, 5, 467, DateTimeKind.Utc)
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
                    SecurityStamp = "cdc21a21-318b-4190-a35c-70e5df9ea56b",
                    ConcurrencyStamp = "12f1d72885d84c62b509135fb4c6c215",
                    PhoneNumber = "0987654321",
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 3, 25, 7, 28, 5, 519, DateTimeKind.Utc)
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
                    SecurityStamp = "01f26687-aac8-470a-ade0-17e00fc98446",
                    ConcurrencyStamp = "5a0347c0fcd04afcafcf7c0186cbb747",
                    PhoneNumber = "0978123456",
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 3, 25, 7, 28, 5, 572, DateTimeKind.Utc)
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
                    SecurityStamp = "c346c74b-28bc-47a4-963a-1dfd9d20e698",
                    ConcurrencyStamp = "289cca9a352241d68433c5fae2da7f0f",
                    PhoneNumber = "0978123356",
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 3, 25, 7, 28, 5, 625, DateTimeKind.Utc)
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
                    SecurityStamp = "ed3d8b90-55be-46d9-ae4d-f10d46005c0a",
                    ConcurrencyStamp = "b36398e53dd3477f9a8d270742882e6b",
                    PhoneNumber = "0912345670",
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 3, 25, 7, 28, 5, 679, DateTimeKind.Utc)
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
                    SecurityStamp = "1250155f-2669-47ad-bb37-162d1ddac80e",
                    ConcurrencyStamp = "a5bdbc63a0754efc947c2db86dd8d7c3",
                    PhoneNumber = "0987654312",
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc)
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
                    CreatedAt = new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc)
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
                    CreatedAt = new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc)
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
                    CreatedAt = new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc)
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
                    CreatedAt = new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc)
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
                    CreatedAt = new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc)
                },
                new Customer
                {
                    CustomerId = 2,
                    FirstName = "Trần Thị",
                    LastName = "Bình",
                    Address = "45 Nguyễn Trãi, Thanh Xuân, Hà Nội",
                    UserId = 11,
                    CreatedAt = new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc)
                }

            );
        }
    }
}

