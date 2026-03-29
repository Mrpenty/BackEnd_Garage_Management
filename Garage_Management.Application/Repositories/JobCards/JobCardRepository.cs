using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Base.Interface;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Application.Repositories.JobCards
{
    /// <summary>
    /// Repository chuyên xử lý truy vấn và thao tác dữ liệu cho entity JobCard.
    /// Kế thừa BaseRepository để tái sử dụng các logic CRUD chung.
    /// </summary>
    public class JobCardRepository : BaseRepository<JobCard>, IJobCardRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Inject DbContext thông qua DI container.
        /// </summary>
        public JobCardRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<bool> HasActiveJobCardAsync(int vehicleId)
        {
            return await _context.JobCards
                .AnyAsync(x =>
                    x.VehicleId == vehicleId &&
                    x.Status != JobCardStatus.Completed);
        }
        /// <summary>
        /// Lấy một JobCard theo Id.
        /// Include các navigation property để tránh lazy loading và null reference.
        /// </summary>
        public async Task<JobCard?> GetByIdAsync(int id)
        {
            return await _context.JobCards
                .Include(j => j.Mechanics)   // Danh sách thợ được phân công
                .Include(j => j.Services)    // Danh sách dịch vụ thực hiện
                    .ThenInclude(s => s.Service)
                .Include(j => j.Services)
                    .ThenInclude(s => s.ServiceTasks)
                        .ThenInclude(st => st.ServiceTask)
                .Include(j => j.SpareParts)  // Danh sách phụ tùng sử dụng
                .Include(j => j.Customer)   // Thông tin khách hàng
                      .ThenInclude(S => S.User)
                .Include(j => j.Vehicle)     // Thông tin xe
                    .ThenInclude(a=> a.Brand) // Thông tin hãng xe
                    .ThenInclude(m => m.Models) // Thông tin model xe
                    .ThenInclude(m => m.VehicleType)
                 .Include(j => j.Supervisor)   // Thông tin supervisor
                 .Include(j => j.Mechanics)    // Thông tin thợ máy
                    .ThenInclude(m => m.Employee)
                .Include(j => j.Logs)        // Lịch sử thao tác / trạng thái
                .Include(jm => jm.Mechanics)    // Thông tin thợ máy
                    .ThenInclude(m => m.Employee)
                .FirstOrDefaultAsync(j => j.JobCardId == id);
        }

        /// <summary>
        /// Lấy danh sách JobCard chưa hoàn thành.
        /// Dùng cho màn hình đang xử lý.
        /// </summary>
        public async Task<(List<JobCard>Items, int TotalCount)> GetActiveAsync(string? search, string? sortBy,string? sortDirection, int page,
    int pageSize)
        {
                   var query = _context.JobCards
            .Include(x => x.Customer)
            .Include(x => x.Vehicle)
                .ThenInclude(v => v.Brand)
            .Include(x => x.Vehicle)
                .ThenInclude(v => v.Model)
            .Include(x => x.Services)
                .ThenInclude(s => s.Service)
            .Where(j => j.Status != JobCardStatus.Completed)
            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                query = query.Where(x =>
                    (x.Customer.FirstName + " " + x.Customer.LastName)
                        .ToLower().Contains(search) ||
                    (x.Vehicle.LicensePlate != null &&
                        x.Vehicle.LicensePlate.ToLower().Contains(search))
                );
            }

            query = sortBy switch
            {
                "StartDate" => sortDirection == "asc"
                    ? query.OrderBy(x => x.StartDate)
                    : query.OrderByDescending(x => x.StartDate),

                "Status" => sortDirection == "asc"
                    ? query.OrderBy(x => x.Status)
                    : query.OrderByDescending(x => x.Status),

                _ => query.OrderByDescending(x => x.StartDate)
            };

            var total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        /// <summary>
        /// Thêm mới một JobCard vào DbContext.
        /// Lưu ý: chưa ghi xuống database cho tới khi gọi SaveChanges.
        /// </summary>
        public async Task AddAsync(JobCard jobCard)
        {
            await _context.JobCards.AddAsync(jobCard);
        }

        /// <summary>
        /// Commit toàn bộ thay đổi xuống database.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        // =========================
        // Các method bên dưới hiện chưa implement.
        // Nếu không sử dụng, nên xoá để tránh nhầm lẫn.
        // =========================

        public Task SaveAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException(); // TODO: implement hoặc xoá
        }

        public void Add(JobCard entity)
        {
            throw new NotImplementedException(); // TODO: implement hoặc xoá
        }

     


        /// <summary>
        /// Trả về IQueryable để Service có thể build query (Select, Where, Paging).
        /// Chưa include navigation property để tối ưu hiệu năng.
        /// </summary>
        public IQueryable<JobCard> GetAll()
        {
            return _context.JobCards;
        }

        public void AddRange(IReadOnlyCollection<JobCard> entities)
        {
            throw new NotImplementedException(); // TODO: implement hoặc xoá
        }

        public async Task<List<JobCard>> GetBySupervisorIdAsync(int supervisorId)
        {
            return await _context.JobCards
                .Include(x => x.Appointment)
                .Include(x => x.Customer)
                .Include(x => x.Vehicle)
                .Include(x => x.Supervisor)
                .Include(x => x.Mechanics)
                .Include(x => x.Services)
                 .ThenInclude(s => s.Service)
                .Include(x => x.Services)
                 .ThenInclude(s => s.ServiceTasks)
                 .ThenInclude(st => st.ServiceTask)
                .Include(x => x.SpareParts)
                .Where(x => x.SupervisorId == supervisorId)
                .ToListAsync();
        }
        public async Task<bool> HasJobCardByAppointmentIdAsync(int? appointmentId)
        {
            return await _context.JobCards
                .AnyAsync(j => j.AppointmentId == appointmentId);
        }
        public async Task<JobCard?> GetWithMechanicsAsync(int jobCardId)
        {
            return await _context.JobCards
                .Include(x => x.Mechanics)
                .FirstOrDefaultAsync(x => x.JobCardId == jobCardId);
        }

        public async Task<bool> IsMechanicAssignedAsync(int jobCardId, int mechanicId)
        {
            return await _context.JobCardMechanics
                .AnyAsync(jcm => jcm.JobCardId == jobCardId && jcm.EmployeeId == mechanicId);
        }

        public async Task<JobCard?> GetByIdWithTasksAsync(int id)
        {
            return await _context.JobCards
                .Include(j => j.Services)
                    .ThenInclude(s => s.ServiceTasks)
                        .ThenInclude(st => st.ServiceTask)
                .Include(j => j.Mechanics)
                    .ThenInclude(m => m.Employee)
                .Include(j => j.Supervisor)
                .Include(j => j.Vehicle)
                    .ThenInclude(v => v.Brand)
                .Include(j => j.Vehicle)
                    .ThenInclude(v => v.Model)
                .Include(j => j.Logs)
                .FirstOrDefaultAsync(j => j.JobCardId == id);
        }
        public async Task<List<JobCard>> GetByWorkBayIdAsync(int workBayId, CancellationToken cancellationToken)
        {
            return await _context.JobCards
                .Include(x => x.Customer)
                .Include(x => x.Vehicle)
                    .ThenInclude(v => v.Brand)
                .Include(x => x.Vehicle)
                    .ThenInclude(v => v.Model)
                .Include(j => j.Mechanics)
                    .ThenInclude(m => m.Employee)
                .Include(x => x.Services)
                    .ThenInclude(s => s.Service)
                .Where(x => x.WorkBayId == workBayId)
                .OrderBy(x => x.Status == JobCardStatus.InProgress ? 0 : 1)
                .ThenBy(x => x.StartDate)
                .ToListAsync(cancellationToken);
        }
        public async Task<List<JobCard>> GetByWorkBayIdsAsync(
    List<int> workBayIds,
    CancellationToken cancellationToken)
        {
            return await _context.JobCards
                .Include(x => x.Customer)
                .Include(x => x.Vehicle)
                    .ThenInclude(v => v.Brand)
                .Include(x => x.Vehicle)
                    .ThenInclude(v => v.Model)
                 .Include(j => j.Mechanics)
                    .ThenInclude(m => m.Employee)
                .Include(x => x.Services)
                    .ThenInclude(s => s.Service)
                .Where(x => x.WorkBayId.HasValue &&
                            workBayIds.Contains(x.WorkBayId.Value))
                .ToListAsync(cancellationToken);
        }
    }
}
