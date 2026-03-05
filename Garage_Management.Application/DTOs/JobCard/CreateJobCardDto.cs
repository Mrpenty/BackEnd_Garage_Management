using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.JobCard
{
    public class CreateJobCardDto
    {
        public int? AppointmentId { get; set; }
        public int CustomerId { get; set; }
        public int VehicleId { get; set; }
        [MaxLength(300)]
        public string? Note { get; set; }
        public int? SupervisorId { get; set; }
        public List<AddServiceToJobCardDto>? Services { get; set; }
    }
}
