using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Iventories
{
    public class InventoryUpdateStatusRequest
    {
        [Required]
        public bool IsActive { get; set; }
    }
}
