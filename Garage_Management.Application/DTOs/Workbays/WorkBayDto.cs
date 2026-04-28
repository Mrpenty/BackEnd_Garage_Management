using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Base.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.Workbays
{
    public class WorkBayDto
    {
        public int Id { get; set; }

        public int BranchId { get; set; }

        public string Name { get; set; }

        public string? Note { get; set; }

        public WorkBayStatus Status { get; set; }

        public int? JobcardId { get; set; }



        public DateTime CreateAt { get; set; }

        public DateTime? UpdateAt { get; set; }

        public DateTime? StartAt { get; set; }

        public DateTime? EndAt { get; set; }
        public List<JobCardListDto> JobCards { get; set; } = new();
    }

    public class CreateWorkBayRequest
    {
        public string Name { get; set; }

        public string? Note { get; set; }

        /// <summary>
        /// Chi nhánh. Non-admin luôn lấy từ token, admin bắt buộc truyền.
        /// </summary>
        public int? BranchId { get; set; }
    }

    public class UpdateWorkBayRequest
    {
        public string Name { get; set; }

        public string? Note { get; set; }

        public WorkBayStatus Status { get; set; }
    }

    public class RebalanceWorkBayQueueResponse
    {
        public int WorkBayId { get; set; }
        public int UpdatedCount { get; set; }
    }
}
