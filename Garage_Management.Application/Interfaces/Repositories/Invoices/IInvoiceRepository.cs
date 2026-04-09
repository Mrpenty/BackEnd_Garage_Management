using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Common.Models.Invoices;
using Garage_Management.Base.Entities;
using Garage_Management.Base.Interface;

namespace Garage_Management.Application.Interfaces.Repositories.Invoices
{
    public interface IInvoiceRepository : IBaseRepository<Invoice>
    {
        Task<Invoice?> GetByIdWithDetailsAsync(int invoiceId, CancellationToken ct = default);
        Task<PagedResult<Invoice>> GetPagedAsync(InvoiceQuery query, CancellationToken ct = default);
        Task<Invoice?> GetByJobCardIdAsync(int jobCardId, CancellationToken ct = default);
        Task SaveAsync(CancellationToken ct);
    }
}
