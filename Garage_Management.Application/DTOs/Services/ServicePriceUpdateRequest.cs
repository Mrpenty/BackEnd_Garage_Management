using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Services
{
    public class ServicePriceUpdateRequest
    {
        [Range(0.01, double.MaxValue, ErrorMessage = "BasePrice phải lớn hơn 0")]
        public decimal BasePrice { get; set; }
    }
}
