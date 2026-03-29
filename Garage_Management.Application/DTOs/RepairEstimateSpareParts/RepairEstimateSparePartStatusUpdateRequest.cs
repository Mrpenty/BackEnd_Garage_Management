using Garage_Management.Base.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.RepairEstimateSpareParts
{
    public class RepairEstimateSparePartStatusUpdateRequest
    {
        [Required]
        public RepairEstimateApprovalStatus Status { get; set; }
    }
}
