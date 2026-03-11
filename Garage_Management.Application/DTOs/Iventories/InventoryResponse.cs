using Garage_Management.Base.Entities.Inventories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.Iventories
{
    public class InventoryResponse
    {
        public int SparePartId { get; set; }
        public string PartName { get; set; } = string.Empty;
        public string? Unit { get; set; }
        public int? SparePartBrandId { get; set; }
        public string? SparePartBrandName { get; set; }
        public decimal? LastPurchasePrice { get; set; }
        public string? ModelCompatible { get; set; }
        public string? VehicleBrand { get; set; }
        public decimal? SellingPrice { get; set; }
        public bool IsActive { get; set; }
    }
}
