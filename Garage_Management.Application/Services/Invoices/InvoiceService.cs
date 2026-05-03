using Garage_Management.Application.DTOs.Invoices;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.Invoices;
using Garage_Management.Application.Interfaces.Services.Auth;
using Garage_Management.Application.Interfaces.Services.Invoices;
using Garage_Management.Application.Services.Auth;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Common.Models.Invoices;
using Garage_Management.Base.Entities;

namespace Garage_Management.Application.Services.Invoices
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IJobCardRepository? _jobCardRepository;
        private readonly ICurrentUserService _currentUser;

        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            IJobCardRepository jobCardRepository,
            ICurrentUserService currentUser)
        {
            _invoiceRepository = invoiceRepository;
            _jobCardRepository = jobCardRepository;
            _currentUser = currentUser;
        }

        public InvoiceService(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
            _jobCardRepository = null;
            _currentUser = new NullCurrentUserService();
        }

        private void EnsureCanAccess(int branchId)
        {
            if (_currentUser.IsAdmin()) return;
            var scoped = _currentUser.GetCurrentBranchId();
            if (scoped.HasValue && scoped.Value == branchId) return;
            throw new UnauthorizedAccessException("Không có quyền truy cập hóa đơn của chi nhánh khác");
        }

        public async Task<InvoiceResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(id, ct);
            if (invoice == null) return null;
            return MapToResponse(invoice);
        }

        public async Task<PagedResult<InvoiceResponse>> GetPagedAsync(InvoiceQuery query, CancellationToken ct = default)
        {
            var pagedResult = await _invoiceRepository.GetPagedAsync(query, ct);
            return new PagedResult<InvoiceResponse>
            {
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                Total = pagedResult.Total,
                PageData = pagedResult.PageData.Select(MapToResponse)
            };
        }

        public async Task<InvoiceResponse> CreateAsync(InvoiceCreateRequest request, CancellationToken ct = default)
        {
            // Kiem tra JobCard da co Invoice chua (quan he 1-1)
            var existing = await _invoiceRepository.GetByJobCardIdAsync(request.JobCardId, ct);
            if (existing != null)
                throw new InvalidOperationException("JobCard already has an invoice.");

            var jobCard = await _jobCardRepository.GetByIdAsync(request.JobCardId);
            if (jobCard == null)
                throw new InvalidOperationException("Không tìm thấy JobCard");
            EnsureCanAccess(jobCard.BranchId);

            var invoice = new Invoice
            {
                BranchId = jobCard.BranchId,
                JobCardId = request.JobCardId,
                InvoiceDate = request.InvoiceDate ?? DateTime.UtcNow,
                ServiceTotal = request.ServiceTotal,
                SparePartTotal = request.SparePartTotal,
                GrandTotal = request.ServiceTotal + request.SparePartTotal,
                PaymentStatus = "Unpaid",
                PaymentMethod = request.PaymentMethod,
                CreatedBy = _currentUser.GetCurrentUserId()
            };

            await _invoiceRepository.AddAsync(invoice, ct);
            await _invoiceRepository.SaveAsync(ct);

            // Load lai voi details de tra ve response day du
            var created = await _invoiceRepository.GetByIdWithDetailsAsync(invoice.InvoiceId, ct);
            return MapToResponse(created!);
        }

        public async Task<InvoiceResponse?> UpdateAsync(int id, InvoiceUpdateRequest request, CancellationToken ct = default)
        {
            var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(id, ct);
            if (invoice == null) return null;
            EnsureCanAccess(invoice.BranchId);

            if (request.InvoiceDate.HasValue)
                invoice.InvoiceDate = request.InvoiceDate.Value;

            if (request.ServiceTotal.HasValue)
                invoice.ServiceTotal = request.ServiceTotal.Value;

            if (request.SparePartTotal.HasValue)
                invoice.SparePartTotal = request.SparePartTotal.Value;

            // Tinh lai GrandTotal neu co thay doi
            if (request.ServiceTotal.HasValue || request.SparePartTotal.HasValue)
                invoice.GrandTotal = invoice.ServiceTotal + invoice.SparePartTotal;

            if (request.PaymentStatus != null)
                invoice.PaymentStatus = request.PaymentStatus;

            if (request.PaymentMethod != null)
                invoice.PaymentMethod = request.PaymentMethod;

            invoice.UpdatedAt = DateTime.UtcNow;
            invoice.UpdatedBy = _currentUser.GetCurrentUserId();

            _invoiceRepository.Update(invoice);
            await _invoiceRepository.SaveAsync(ct);

            var updated = await _invoiceRepository.GetByIdWithDetailsAsync(id, ct);
            return MapToResponse(updated!);
        }

        private static InvoiceResponse MapToResponse(Invoice invoice)
        {
            string? customerName = null;
            string? licensePlate = null;
            var services = new List<InvoiceServiceItemDto>();
            var spareParts = new List<InvoiceSparePartItemDto>();

            if (invoice.JobCard != null)
            {
                if (invoice.JobCard.Customer != null)
                {
                    customerName = $"{invoice.JobCard.Customer.FirstName} {invoice.JobCard.Customer.LastName}".Trim();
                }
                licensePlate = invoice.JobCard.Vehicle?.LicensePlate;

                if (invoice.JobCard.Services != null)
                {
                    services = invoice.JobCard.Services
                        .Select(s => new InvoiceServiceItemDto
                        {
                            JobCardServiceId = s.JobCardServiceId,
                            ServiceId = s.ServiceId,
                            ServiceName = s.Service?.ServiceName ?? string.Empty,
                            Description = s.Description,
                            Price = s.Price,
                            Status = s.Status.ToString()
                        })
                        .ToList();
                }

                if (invoice.JobCard.SpareParts != null)
                {
                    spareParts = invoice.JobCard.SpareParts
                        .Select(p => new InvoiceSparePartItemDto
                        {
                            SparePartId = p.SparePartId,
                            PartCode = p.Inventory?.PartCode,
                            PartName = p.Inventory?.PartName ?? string.Empty,
                            Quantity = p.Quantity,
                            UnitPrice = p.UnitPrice,
                            TotalAmount = p.TotalAmount,
                            IsUnderWarranty = p.IsUnderWarranty,
                            Note = p.Note
                        })
                        .ToList();
                }
            }

            return new InvoiceResponse
            {
                InvoiceId = invoice.InvoiceId,
                JobCardId = invoice.JobCardId,
                InvoiceDate = invoice.InvoiceDate,
                ServiceTotal = invoice.ServiceTotal,
                SparePartTotal = invoice.SparePartTotal,
                GrandTotal = invoice.GrandTotal,
                PaymentStatus = invoice.PaymentStatus,
                PaymentMethod = invoice.PaymentMethod,
                CustomerName = customerName,
                VehicleLicensePlate = licensePlate,
                CreatedAt = invoice.CreatedAt,
                UpdatedAt = invoice.UpdatedAt,
                Services = services,
                SpareParts = spareParts
            };
        }
    }
}
