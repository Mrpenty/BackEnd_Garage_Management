using Garage_Management.Application.DTOs.Employee;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Services.Accounts;
using Garage_Management.Application.Interfaces.Services.Auth;

namespace Garage_Management.Application.Services.Accounts
{
    public class MechanicService : IMechanicService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICurrentUserService _currentUser;

        public MechanicService(IEmployeeRepository employeeRepository, ICurrentUserService currentUser)
        {
            _employeeRepository = employeeRepository;
            _currentUser = currentUser;
        }

        public async Task<List<MechanicStatusResponse>> GetAllMechanicsAsync()
        {
            int? branchFilter = _currentUser.IsAdmin() ? null : _currentUser.GetCurrentBranchId();

            var mechanics = await _employeeRepository.GetAllMechanicsAsync(branchFilter);

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
