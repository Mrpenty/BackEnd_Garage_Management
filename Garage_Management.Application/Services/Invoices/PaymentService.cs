using Garage_Management.Application.DTOs.Invoices;
using Garage_Management.Application.Interfaces.Repositories.Invoices;
using Garage_Management.Application.Interfaces.Services.Invoices;
using Garage_Management.Base.Common.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using VNPAY;
using VNPAY.Models.Enums;
using VNPAY.Models.Exceptions;

namespace Garage_Management.Application.Services.Invoices
{
    public class PaymentService : IPaymentService
    {
        private readonly IVnpayClient _vnpayClient;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IConfiguration _configuration;

        public PaymentService(IVnpayClient vnpayClient, IInvoiceRepository invoiceRepository, IConfiguration configuration)
        {
            _vnpayClient = vnpayClient;
            _invoiceRepository = invoiceRepository;
            _configuration = configuration;
        }

        public async Task<PaymentResponse> CreatePaymentUrlAsync(CreatePaymentRequest request, CancellationToken ct = default)
        {
            var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(request.InvoiceId);
            if(invoice == null)
            {
                throw new InvalidOperationException("Không tìm thấy hóa đơn");
            }
            if(invoice.PaymentStatus == PaymentStatus.Paid.ToString())
            {
                throw new InvalidOperationException("Hóa đơn đã được thanh toán");
            }
            if(invoice.GrandTotal < 5000 || invoice.GrandTotal > 1000000000)
            {
                throw new InvalidOperationException("Số tiền thanh toán phải từ 5,000 VND đến 1,000,000,000 VND.");
            }
            var moneyToPay = (long)invoice.GrandTotal;
            var description = $"Thanh toán hóa đơn #{invoice.InvoiceId}";

            var paymentUrlInfo = _vnpayClient.CreatePaymentUrl(moneyToPay, description, BankCode.ANY);

            return new PaymentResponse
            {
                InvoiceId = invoice.InvoiceId,
                PaymentUrl = paymentUrlInfo.Url,
                PaymentStatus = PaymentStatus.Unpaid.ToString(),
                Amount = invoice.GrandTotal
            };
        }

        public async Task<PaymentResponse> ProcessPaymentCallbackAsync(HttpRequest httpRequest, CancellationToken ct = default)
        {
            try
            {
                var paymentResult = _vnpayClient.GetPaymentResult(httpRequest);

                // Parse InvoiceId tu Description (format: "Thanh toán hóa đơn #<InvoiceId>")
                var description = paymentResult.Description ?? "";
                var hashIndex = description.LastIndexOf('#');
                var invoiceIdStr = hashIndex >= 0 ? description[(hashIndex + 1)..].Trim() : "";

                if (!int.TryParse(invoiceIdStr, out var invoiceId))
                    throw new InvalidOperationException(
                        $"Cannot parse InvoiceId. PaymentId={paymentResult.PaymentId}, Description='{description}'");

                var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(invoiceId);
                if (invoice == null)
                    throw new InvalidOperationException(
                        $"Invoice not found. Parsed InvoiceId={invoiceId}, PaymentId={paymentResult.PaymentId}, Description='{description}'");

                invoice.PaymentStatus = PaymentStatus.Paid.ToString();
                invoice.PaymentMethod = $"VNPAY - {paymentResult.CardType}";

                invoice.JobCard.Status = JobCardStatus.Completed;

                if (invoice.JobCard.Appointment != null)
                {
                    invoice.JobCard.Appointment.Status = AppointmentStatus.Completed;
                }

                _invoiceRepository.Update(invoice);
                await _invoiceRepository.SaveAsync(ct);

                return new PaymentResponse
                {
                    InvoiceId = invoice.InvoiceId,
                    PaymentStatus = invoice.PaymentStatus,
                    Amount = invoice.GrandTotal,
                    TransactionId = paymentResult.VnpayTransactionId.ToString(),
                    Message = "Payment successful"
                };
            }
            catch (VnpayException)
            {
                // VNPAY bao thanh toan that bai
                return new PaymentResponse
                {
                    PaymentStatus = PaymentStatus.Failed.ToString(),
                    Message = "Payment failed"
                };
            }
        }

        public async Task<BankTransferResponse> CreateBankTransferQrAsync(int invoiceId, CancellationToken ct = default)
        {
            var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(invoiceId);
            if (invoice == null)
                throw new InvalidOperationException("Không tìm thấy hóa đơn.");

            if (invoice.PaymentStatus == PaymentStatus.Paid.ToString())
                throw new InvalidOperationException("Hóa đơn đã được thanh toán.");

            var bankConfig = _configuration.GetSection("BankTransfer");
            var bankId = bankConfig["BankId"]!;
            var accountNumber = bankConfig["AccountNumber"]!;
            var accountName = bankConfig["AccountName"]!;
            var template = bankConfig["Template"] ?? "compact2";

            var amount = (long)invoice.GrandTotal;
            var transferContent = $"SEVQR TT HD {invoice.InvoiceId}";

           //Tạo QR của Sepay
            var qrUrl = $"https://qr.sepay.vn/img?acc={accountNumber}&bank={bankId}" +
                        $"&amount={amount}" +
                        $"&des={Uri.EscapeDataString(transferContent)}" +
                        $"&template={template}";

            return new BankTransferResponse
            {
                InvoiceId = invoice.InvoiceId,
                Amount = invoice.GrandTotal,
                QrCodeUrl = qrUrl,
                BankName = bankId,
                AccountNumber = accountNumber,
                AccountName = accountName,
                TransferContent = transferContent
            };
        }
        public async Task<bool> ProcessSePayWebhookAsync(SePayWebhookRequest request, CancellationToken ct = default)
        {
            // Noi dung chuyen khoan co dang: "TT HD {invoiceId}"
            var content = request.Content ?? "";
            var prefix = "SEVQR TT HD ";
            var idx = content.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
            if (idx < 0 || !int.TryParse(content[(idx + prefix.Length)..].Trim(), out var invoiceId))
                return false;

            var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(invoiceId);
            if (invoice == null || invoice.PaymentStatus == PaymentStatus.Paid.ToString())
                return false;

            invoice.PaymentStatus = PaymentStatus.Paid.ToString();
            invoice.PaymentMethod = "BankTransfer";

            invoice.JobCard.Status = JobCardStatus.Completed;

            if (invoice.JobCard.Appointment != null)
            {
                invoice.JobCard.Appointment.Status = AppointmentStatus.Completed;
            }

            _invoiceRepository.Update(invoice);
            await _invoiceRepository.SaveAsync(ct);

            return true;
        }
    }
}
