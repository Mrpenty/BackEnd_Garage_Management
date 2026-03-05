using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.Vehicles.VehicleModel
{
    public class VehicleModelCreateRequest
    {
        [Required]
        public int TypeId { get; set; }
        [Required]
        public int BrandId { get; set; }
        [Required]
        public string ModelName { get; set; }
        [Required]
        public bool isActive { get; set; }

    }
}
