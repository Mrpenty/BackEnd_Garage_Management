using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.Vehicles.VehicleModel
{
    public class VehicleModelCreateRequest
    {
        public int BrandId { get; set; }
        public string ModelName { get; set; }
    }
}
