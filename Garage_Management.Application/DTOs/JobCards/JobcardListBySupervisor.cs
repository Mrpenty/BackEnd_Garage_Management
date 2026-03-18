using Garage_Management.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.JobCards
{
    public class JobcardListBySupervisor
    {
            public int JobCardId { get; set; }

            public int? AppointmentId { get; set; }
            public object? Appointment { get; set; }

            public int CustomerId { get; set; }
            public object? Customer { get; set; }

            public int VehicleId { get; set; }
            public object? Vehicle { get; set; }

            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }

            public int Status { get; set; }
            public string? Note { get; set; }

            public int SupervisorId { get; set; }
            public object? Supervisor { get; set; }

            public List<object> Services { get; set; } = new();

        }
    }
