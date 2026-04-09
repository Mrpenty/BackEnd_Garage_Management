using Garage_Management.Application.DTOs.Invoices;
using Garage_Management.Application.Interfaces.Repositories.Invoices;
using Garage_Management.Application.Interfaces.Services.Invoices;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Common.Models.Invoices;
using Garage_Management.Base.Entities;

namespace Garage_Management.Application.Services.Invoices
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceService(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
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

            var invoice = new Invoice
            {
                JobCardId = request.JobCardId,
                InvoiceDate = request.InvoiceDate ?? DateTime.UtcNow,
                ServiceTotal = request.ServiceTotal,
                SparePartTotal = request.SparePartTotal,
                GrandTotal = request.ServiceTotal + request.SparePartTotal,
                PaymentStatus = "Unpaid",
                PaymentMethod = request.PaymentMethod
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

            _invoiceRepository.Update(invoice);
            await _invoiceRepository.SaveAsync(ct);

            var updated = await _invoiceRepository.GetByIdWithDetailsAsync(id, ct);
            return MapToResponse(updated!);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null) return false;

            _invoiceRepository.Delete(invoice);
            await _invoiceRepository.SaveAsync(ct);
            return true;
        }

        private static InvoiceResponse MapToResponse(Invoice invoice)
        {
            string? customerName = null;
            string? licensePlate = null;

            if (invoice.JobCard != null)
            {
                if (invoice.JobCard.Customer != null)
                {
                    customerName = $"{invoice.JobCard.Customer.FirstName} {invoice.JobCard.Customer.LastName}".Trim();
                }
                licensePlate = invoice.JobCard.Vehicle?.LicensePlate;
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
                UpdatedAt = invoice.UpdatedAt
            };
        }
    }
}
