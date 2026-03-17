using Garage_Management.Base.Entities.Vehiclies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Base.Entities.Inventories
{
    public class InventoryVehicleModel
    {
        public int SparePartId { get; set; }
        public Inventory Inventory { get; set; } = null!;
        public int VehicleModelId { get; set; }
        public VehicleModel VehicleModel { get; set; } = null!;
    }
}
