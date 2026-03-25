using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.JobCards
{
    public class AssignWorkBayRequestDto
    {
        public int JobCardId { get; set; }
        public int WorkBayId { get; set; }
    }

}
