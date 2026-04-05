using Garage_Management.Base.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.RepairEstimateServices
{
    public class RepairEstimateServiceStatusUpdateRequest
    {
        [Required]
        public RepairEstimateApprovalStatus Status { get; set; }
    }
}
