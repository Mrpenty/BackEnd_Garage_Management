using Garage_Management.Base.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.JobCards
{
    public class UpdateJobCardProgressDto
    {
        /// <summary>
        /// Trạng thái hiện tại (In Progress, Waiting Parts, Completed Step)
        /// </summary>
        public JobCardStatus? StatusJobCard { get; set; }

        /// <summary>
        /// Phần trăm hoàn thành (0-100)
        /// </summary>
        [Range(0, 100)]
        public int? ProgressPercentage { get; set; }

        /// <summary>
        /// Các bước đã hoàn thành (mô tả text)
        /// </summary>
        //public string? CompletedSteps { get; set; }

        /// <summary>
        /// Ghi chú tiến độ hoặc comments thêm
        /// </summary>
        public string? ProgressNotes { get; set; }

        /// <summary>
        /// Các faults mới phát hiện (mô tả)
        /// </summary>
        public string? AdditionalFaults { get; set; }

        /// <summary>
        /// Cập nhật trạng thái các services
        /// </summary>
        public List<UpdateServiceStatusDto>? ServiceUpdates { get; set; }
        public List<ServiceTaskUpdateDto>? ServiceTaskUpdates { get; set; }
    }
}