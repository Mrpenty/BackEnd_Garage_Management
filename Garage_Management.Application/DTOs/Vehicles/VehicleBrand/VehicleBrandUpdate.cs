using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.Vehicles.VehicleBrand
{
    public class VehicleBrandUpdate
    {
        [Required]
        public string BrandName { get; set; }

    }
}
