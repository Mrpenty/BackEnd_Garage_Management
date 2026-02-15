using Garage_Management.Base.Common.Enums;
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
        public int VehicleId { get; set; }
        public ServiceStatus Status { get; set; }
        public DateTime StartDate { get; set; }
    }

}
