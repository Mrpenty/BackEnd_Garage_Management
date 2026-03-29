using Garage_Management.Application.DTOs.JobCardMechanics;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Repositories.JobCards
{
    public class JobCardMechanicRepository : BaseRepository<JobCardMechanic>, IJobCardMechanicRepository
    {
        private readonly AppDbContext _context; 
        public JobCardMechanicRepository(AppDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }


        public async Task<PagedResult<JobCardMechanicDto>> GetJobCardsByEmployeeIdAsync(int employeeId, ParamQuery param)
        {
            var query = _context.JobCardMechanics
                .AsNoTracking()
                .Where(jm => jm.EmployeeId == employeeId)
                .Include(jm => jm.JobCard)
                    .ThenInclude(jc => jc.Customer)
                .Include(jm => jm.JobCard)
                    .ThenInclude(jc => jc.Vehicle)
                .Include(jm => jm.JobCard)
                    .ThenInclude(jc => jc.Supervisor)
                .Include(jm => jm.JobCard)
                    .ThenInclude(jc => jc.Appointment)
                .Include(jm => jm.JobCard)
                    .ThenInclude(jc => jc.Services)
                    .ThenInclude(jc =>jc.ServiceTasks)
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(param.Search))
            {
                var search = param.Search.ToLower().Trim();
                query = query.Where(jm =>
                    jm.JobCard.Customer.FirstName.ToLower().Contains(search) ||
                    jm.JobCard.Customer.LastName.ToLower().Contains(search) ||
                    jm.JobCard.Vehicle.LicensePlate.ToLower().Contains(search) ||
                    jm.JobCard.JobCardId.ToString().Contains(search));
            }

            // Filter (nếu cần mở rộng sau)

            // Sorting
            if (!string.IsNullOrEmpty(param.OrderBy))
            {
                query = param.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(GetOrderByExpression(param.OrderBy))
                    : query.OrderBy(GetOrderByExpression(param.OrderBy));
            }
            else
            {
                query = query.OrderByDescending(jm => jm.AssignedAt); // Mặc định mới nhất trước
            }

            var total = await query.CountAsync();

            var data = await query
                .Skip((param.Page - 1) * param.PageSize)
                .Take(param.PageSize)
                .Select(jm => new JobCardMechanicDto
                {
                    JobCardId = jm.JobCardId,
                    EmployeeId = jm.EmployeeId,
                    AssignedAt = jm.AssignedAt,
                    StartedAt = jm.StartedAt,
                    CompletedAt = jm.CompletedAt,
                    MechanicAssignmenStatus = jm.Status,
                    Note = jm.Note,

                    
                    // JobCard
                    JobCardStartDate = jm.JobCard.StartDate,
                    JobCardEndDate = jm.JobCard.EndDate,
                    JobCardStatus = jm.JobCard.Status,
                    ProgressPercentage = jm.JobCard.ProgressPercentage,
                    ProgressNotes = jm.JobCard.ProgressNotes,
                    JobCardDescription = jm.JobCard.Note, 
                    Supervisor = jm.JobCard.Supervisor != null ? $"{jm.JobCard.Supervisor.FirstName} {jm.JobCard.Supervisor.LastName}".Trim() : string.Empty,

                    // Customer
                    CustomerId = jm.JobCard.CustomerId,
                    CustomerFullName = $"{jm.JobCard.Customer.FirstName} {jm.JobCard.Customer.LastName}".Trim(),
                    CustomerPhone = jm.JobCard.Customer.User.PhoneNumber,  

                    // Vehicle
                    VehicleId = jm.JobCard.VehicleId,
                    LicensePlate = jm.JobCard.Vehicle.LicensePlate,
                    VehicleBrand = jm.JobCard.Vehicle.Brand.BrandName,
                    VehicleModel = jm.JobCard.Vehicle.Model.ModelName,

                    // Appointment
                    AppointmentId = jm.JobCard.AppointmentId,
                    AppointmentDate = jm.JobCard.Appointment.AppointmentDateTime,
                    AppointmentNote = jm.JobCard.Appointment.Description,

                    Services = jm.JobCard.Services
                     .Where(s => s.Service != null)
                     .Select(s => new ServiceJobCardMechanicResponse
                     {
                           ServiceId = s.ServiceId,
                           ServiceName = s.Service.ServiceName,
                           TotalEstimateMinute = s.ServiceTasks.Sum(st => st.ServiceTask.EstimateMinute),
                         ServiceTasks = s.ServiceTasks.Select(st => new ServiceTaskJobCardMechanicResponse
                           {
                               ServiceTaskId = st.ServiceTaskId,
                               TaskName = st.ServiceTask.TaskName,
                               TaskOrder = st.ServiceTask.TaskOrder,
                               EstimateMinute = st.ServiceTask.EstimateMinute,
                               ServiceTaskStatusName = st.Status.ToString(),
                               Note = st.Note
                           }).ToList(),
                           Description = s.Description,
                           CreatedAt = s.CreatedAt,
                           UpdatedAt = s.UpdatedAt,
                           ServiceStatusName = s.Status.ToString(),
                     })
                      .ToList()
                  })
                .ToListAsync();

            if (data == null || data.Count == 0)
                return new PagedResult<JobCardMechanicDto>
                {
                    PageData = new List<JobCardMechanicDto>(),
                    Page = param.Page,
                    PageSize = param.PageSize,
                    Total = 0
                };

            return new PagedResult<JobCardMechanicDto>
            {
                PageData = data,
                Page = param.Page,
                PageSize = param.PageSize,
                Total = total
            };
        }

        // Helper để OrderBy động (có thể mở rộng)
        private Expression<Func<JobCardMechanic, object>> GetOrderByExpression(string orderBy)
        {
            return orderBy.ToLower() switch
            {
                "startdate" => jm => jm.JobCard.StartDate,
                "assignedat" => jm => jm.AssignedAt,
                "status" => jm => jm.Status,
                _ => jm => jm.AssignedAt
            };
        }
    }
}