using Garage_Management.Application.DTOs.Invoices;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Common.Models.Invoices;

namespace Garage_Management.Application.Interfaces.Services.Invoices
{
    public interface IInvoiceService
    {
        Task<InvoiceResponse?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<PagedResult<InvoiceResponse>> GetPagedAsync(InvoiceQuery query, CancellationToken ct = default);
        Task<InvoiceResponse> CreateAsync(InvoiceCreateRequest request, CancellationToken ct = default);
        Task<InvoiceResponse?> UpdateAsync(int id, InvoiceUpdateRequest request, CancellationToken ct = default);
    }
}
