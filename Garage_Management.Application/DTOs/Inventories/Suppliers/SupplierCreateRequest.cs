using Garage_Management.Base.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Inventories.Suppliers
{
    public class SupplierCreateRequest
    {
        [Required]
        [MaxLength(200)]
        public string SupplierName { get; set; } = string.Empty;

        [Required]
        public SupplierType SupplierType { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? TaxCode { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
