using Garage_Management.Application.DTOs.JobCard;
using Garage_Management.Application.Interfaces.Repositories;
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
                .Include(j => j.SpareParts)  // Danh sách phụ tùng sử dụng
                .Include(j => j.Logs)        // Lịch sử thao tác / trạng thái
                .FirstOrDefaultAsync(j => j.JobCardId == id);
        }

        /// <summary>
        /// Lấy danh sách JobCard chưa hoàn thành.
        /// Dùng cho màn hình đang xử lý.
        /// </summary>
        public async Task<List<JobCard>> GetActiveAsync(
    string? search,
    string? sortBy,
    string? sortDirection)
        {
            var query = _context.JobCards
                .Include(x => x.Customer)
                .Include(x => x.Vehicle)
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

            return await query.ToListAsync();
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

        public async Task<List<JobcardListBySupervisor>> GetBySupervisorIdAsync(int supervisorId)
        {
            var jobCards = await _context.JobCards
                .Include(x => x.Customer)
                .Include(x => x.Vehicle)
                .Include(x => x.Supervisor)
                .Include(x => x.Appointment)
                .Include(x => x.Mechanics)
                .Include(x => x.Services)
                .Include(x => x.SpareParts)
                .Where(x => x.SupervisorId == supervisorId)
                .ToListAsync();

            return jobCards.Select(j => new JobcardListBySupervisor
            {
                JobCardId = j.JobCardId,

                AppointmentId = j.AppointmentId,
                Appointment = j.Appointment == null ? null : new
                {
                    j.Appointment.AppointmentDateTime
                },

                CustomerId = j.CustomerId,
                Customer = j.Customer == null ? null : new
                {
                    j.Customer.CustomerId,
                    j.Customer.FirstName,
                    j.Customer.LastName,
                },

                VehicleId = j.VehicleId,
                Vehicle = j.Vehicle == null ? null : new
                {
                    j.Vehicle.VehicleId,
                    j.Vehicle.LicensePlate,
                    j.Vehicle.Brand
                },

                StartDate = j.StartDate,
                EndDate = j.EndDate,
                Status = (int)j.Status,
                Note = j.Note,

                SupervisorId = (int)j.SupervisorId,
                Supervisor = j.Supervisor == null ? null : new
                {
                    j.Supervisor.UserId,
                    j.Supervisor.FirstName,
                    j.Supervisor.LastName,
                },

                Mechanics = j.Mechanics.Select(m => new
                {
                    m.EmployeeId
                }).Cast<object>().ToList(),

                Services = j.Services.Select(s => new
                {
                    s.ServiceId,
                    s.Service
                }).Cast<object>().ToList(),

                SpareParts = j.SpareParts.Select(sp => new
                {
                    sp.SparePartId
                }).Cast<object>().ToList(),

                CreatedAt = j.CreatedAt,
                CreatedBy = (int)j.CreatedBy
            }).ToList();
        }
        public async Task<bool> HasJobCardByAppointmentIdAsync(int? appointmentId)
        {
            return await _context.JobCards
                .AnyAsync(j => j.AppointmentId == appointmentId);
        }
    }
}
