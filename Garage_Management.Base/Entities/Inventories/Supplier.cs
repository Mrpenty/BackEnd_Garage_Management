using Garage_Management.Base.Common.Base;
using Garage_Management.Base.Common.Enums;
using System.Collections.Generic;

namespace Garage_Management.Base.Entities.Inventories
{
    public class Supplier : AuditableEntity
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = null!;
        public SupplierType SupplierType { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? TaxCode { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();
    }
}
