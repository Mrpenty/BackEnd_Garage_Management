using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.Vehicles.VehicleModel
{
    public class VehicleModelUpdate
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "TypeId phải lớn hơn 0")]
        public int TypeId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "BrandId phải lớn hơn 0")]
        public int BrandId { get; set; }
        [Required]
        public string ModelName { get; set; } = string.Empty;
        [Required]
        public bool isActive { get; set; }

    }
}
