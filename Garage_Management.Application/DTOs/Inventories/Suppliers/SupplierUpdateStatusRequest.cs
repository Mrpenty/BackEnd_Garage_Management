using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Inventories.Suppliers
{
    public class SupplierUpdateStatusRequest
    {
        [Required]
        public bool IsActive { get; set; }
    }
}
