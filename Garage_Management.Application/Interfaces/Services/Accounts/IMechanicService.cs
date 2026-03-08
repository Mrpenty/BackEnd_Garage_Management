using Garage_Management.Application.DTOs.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Interfaces.Services.Accounts
{
    public interface IMechanicService
    {
       
            Task<List<MechanicStatusResponse>> GetAllMechanicsAsync();
        
    }
}
