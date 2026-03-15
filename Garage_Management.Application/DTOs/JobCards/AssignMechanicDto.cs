using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.JobCards
{
    public class AssignMechanicDto
    {
        public int MechanicId { get; set; }
        [MaxLength(300)]
        public string? Note { get; set; }
    }

}
