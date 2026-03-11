  using Garage_Management.Application.DTOs.Employee;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Services.Accounts;
    using Garage_Management.Base.Common.Enums;
    using Garage_Management.Base.Data;
    using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Services.Accounts
{


    public class MechanicService : IMechanicService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public MechanicService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<List<MechanicStatusResponse>> GetAllMechanicsAsync()
        {
            var mechanics = await _employeeRepository.GetAllMechanicsAsync();

            return mechanics.Select(m => new MechanicStatusResponse
            {
                EmployeeId = m.EmployeeId,
                EmployeeCode = m.EmployeeCode,
                FullName = m.FullName,
                Status = m.Status
            }).ToList();
        }
    }
}
