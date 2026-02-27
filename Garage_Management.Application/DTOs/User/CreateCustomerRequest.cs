using Garage_Management.Application.DTOs.Vehicles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.User
{
    // Request từ form tạo khách hàng
    public class CreateCustomerRequest
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        public string? Address { get; set; }

        // Danh sách xe (có thể thêm nhiều xe)
        //public List<VehicleCreateRequest>? Vehicles { get; set; }
    }

    // DTO trả về
    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Address { get; set; }
        public int? UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<VehicleDto> Vehicles { get; set; } = new();
    }

    public class VehicleDto
    {
        public int VehicleId { get; set; }
        public string LicensePlate { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }
    }
}
