using Garage_Management.Application.DTOs.Invoices;

namespace Garage_Management.Application.Interfaces.Services.Invoices
{
    public interface IPaymentService
    {
        Task<PaymentResponse> CreatePaymentUrlAsync(CreatePaymentRequest request, CancellationToken ct = default);

        Task<PaymentResponse> ProcessPaymentCallbackAsync(Microsoft.AspNetCore.Http.HttpRequest httpRequest, CancellationToken ct = default);

        /// <summary>
        /// Tao QR code chuyen khoan ngan hang (VietQR) cho Invoice
        /// </summary>
        Task<BankTransferResponse> CreateBankTransferQrAsync(int invoiceId, CancellationToken ct = default);

        /// <summary>
        /// Xu ly webhook tu SePay khi co chuyen khoan vao tai khoan
        /// </summary>
        Task<bool> ProcessSePayWebhookAsync(SePayWebhookRequest request, CancellationToken ct = default);
    }
}
