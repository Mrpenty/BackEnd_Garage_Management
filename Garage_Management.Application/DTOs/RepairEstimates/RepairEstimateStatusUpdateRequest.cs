using Garage_Management.Base.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.RepairEstimates
{
    public class RepairEstimateStatusUpdateRequest
    {
        [Required]
        public RepairEstimateApprovalStatus Status { get; set; }
    }
}
