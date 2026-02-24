using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.Vehicles.VehicleBrand
{
    public class VehicleBrandResponse
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public bool isActive { get; set; }


    }
}
