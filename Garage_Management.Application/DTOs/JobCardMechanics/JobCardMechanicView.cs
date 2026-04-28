using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.JobCardMechanics
{
    public class JobCardMechanicView
    {
        public int? MechanicId { get; set; }
        public string MechanicName { get; set; } = null!;

        public DateTime AssignedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }            
    
    }
}
