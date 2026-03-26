using Garage_Management.Base.Common.Enums;

namespace Garage_Management.Application.DTOs.Inventories.Suppliers
{
    public class SupplierResponse
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public SupplierType SupplierType { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? TaxCode { get; set; }
        public bool IsActive { get; set; }
    }
}
