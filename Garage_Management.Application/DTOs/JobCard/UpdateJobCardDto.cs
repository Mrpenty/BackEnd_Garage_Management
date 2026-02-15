using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.JobCard
{
    public class UpdateJobCardDto
    {
        public string? Note { get; set; }

        public int? SupervisorId { get; set; }

        public DateTime? EndDate { get; set; }
    }

}
