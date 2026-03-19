using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.Inventories.SparePartCategories
{
    public class SparePartUpdateStatusRequest
    {
        [Required]
        public bool IsActive {  get; set; }
    }
}
