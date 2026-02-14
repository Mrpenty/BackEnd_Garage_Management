using Garage_Management.Application.DTOs.Appointments;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services
{
    /*
     * Author: KhanhDV
     * Created Date: 13-02-2026
    */
    public interface IAppointmentService
    {
        Task<AppointmentResponse?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<PagedResult<AppointmentResponse>> GetByCustomerIdAsync(int page, int pageSize, int customerId, CancellationToken ct = default);
        Task<PagedResult<AppointmentResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<AppointmentResponse> CreateAsync(AppointmentCreateRequest request, CancellationToken ct = default);
        Task<AppointmentResponse?> UpdateAsync(int id, AppointmentUpdateRequest request, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
