using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Base.Interface;

namespace Garage_Management.Application.Interfaces.Repositories.JobCards
{
    public interface IJobCardRepository : IBaseRepository<JobCard>
    {
        Task<(List<JobCard> Items, int TotalCount)> GetActiveAsync(string? search, string? sortBy, string? sortDirection,int page,
            int pageSize);
        Task SaveChangesAsync();
        Task<bool> HasActiveJobCardAsync(int vehicleId);
        Task<List<JobCard>> GetBySupervisorIdAsync(int supervisorId);
        Task<List<JobCard>> GetByCustomerIdAsync(int customerId);
       
        Task<bool> HasJobCardByAppointmentIdAsync(int? appointmentId);
        Task<JobCard?> GetWithMechanicsAsync(int jobCardId);
        Task<bool> IsMechanicAssignedAsync(int jobCardId, int mechanicId);
        Task<JobCard?> GetByIdWithTasksAsync(int id);
        Task<List<JobCard>> GetByWorkBayIdAsync(int workBayId, CancellationToken cancellationToken);
        Task<List<JobCard>> GetByWorkBayIdsAsync(List<int> workBayIds, CancellationToken cancellationToken);

    }
}
