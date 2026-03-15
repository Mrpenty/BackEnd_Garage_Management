using Garage_Management.Application.DTOs.Services;
using Garage_Management.Application.DTOs.Vehicles;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.JobCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.JobCards
{
    public class JobCardListDto
    {
        public int JobCardId { get; set; }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;

        public VehicleListDto Vehicle { get; set; } = null!;

        public JobCardStatus Status { get; set; }
        public DateTime StartDate { get; set; }

        public List<ServiceResponse> Services { get; set; } = new();
    }

}
