using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.Vehicles.VehicleModel
{
    public class VehicleModelResponse
    {
        public int TypeId {  get; set; }
        public int BrandId { get; set; }
        public int ModelId { get; set; }
        public string ModelName {  get; set; }
        public bool isActive { get; set; }

    }
}
