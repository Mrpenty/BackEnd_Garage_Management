using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.JobCards
{
    public class AddServiceToJobCardDto
    {
        public int ServiceId { get; set; }
        public string? Description { get; set; }
    }

}
