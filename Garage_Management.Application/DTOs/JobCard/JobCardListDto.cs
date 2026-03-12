using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.JobCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.JobCard
{
    public class JobCardListDto
    {
        public int JobCardId { get; set; }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;

        public int VehicleId { get; set; }
        public string? LicensePlate { get; set; }

        public JobCardStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public ICollection<JobCardService> Service { get; internal set; }
    }

}
