using Garage_Management.Application.DTOs.Invoices;
using Garage_Management.Application.Interfaces.Services.Invoices;
using Garage_Management.Application.Services.Invoices;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Tao URL thanh toan VNPAY cho Invoice
        /// </summary>
        [HttpPost("create")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PaymentResponse>>> CreatePayment(
            [FromBody] CreatePaymentRequest request,
            CancellationToken ct = default)
        {
            var result = await _paymentService.CreatePaymentUrlAsync(request, ct);
            return Ok(ApiResponse<PaymentResponse>.SuccessResponse(result, "Payment URL created successfully"));
        }

        /// <summary>
        /// Callback URL tu VNPAY - xu ly ket qua thanh toan
        /// VNPAY se goi GET request den endpoint nay sau khi khach hang thanh toan
        /// </summary>
        [HttpGet("callback")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<PaymentResponse>>> PaymentCallback(CancellationToken ct = default)
        {
            var result = await _paymentService.ProcessPaymentCallbackAsync(Request, ct);
            return Ok(ApiResponse<PaymentResponse>.SuccessResponse(result, result.Message ?? "OK"));
        }

        /// <summary>
        /// Tao QR code chuyen khoan ngan hang (VietQR) cho Invoice
        /// Khach quet QR bang app ngan hang bat ky de chuyen khoan
        /// </summary>
        [HttpGet("bank-transfer/{invoiceId:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<BankTransferResponse>>> CreateBankTransferQr(
            int invoiceId,
            CancellationToken ct = default)
        {
            var result = await _paymentService.CreateBankTransferQrAsync(invoiceId, ct);
            return Ok(ApiResponse<BankTransferResponse>.SuccessResponse(result, "QR code created successfully"));
        }

        /// <summary>
        /// Webhook tu SePay - xu ly khi co chuyen khoan vao tai khoan
        /// SePay se goi POST request den endpoint nay sau khi phat hien giao dich
        /// </summary>
        [HttpPost("sepay-webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> SePayWebhook(
            [FromBody] SePayWebhookRequest request,
            CancellationToken ct = default)
        {
            var ok = await _paymentService.ProcessSePayWebhookAsync(request, ct);
            if (!ok)
                return BadRequest(new { success = false, message = "Không khớp invoice từ nội dung chuyển khoản" });
            return Ok(new { success = true });
        }
        /// <summary>
        /// Thanh toán bằng tiền mặt tại quầy lễ tân
        /// </summary>
        [HttpPost("cash-payment")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PaymentResponse>>> CashPayment(int invoiceId, CancellationToken ct = default)
        {
            var result = await _paymentService.UpdateAfterPaymentSuccessAsync(invoiceId, ct);
            if (result)
            {
                return Ok(ApiResponse<string>.SuccessResponse( "Cập nhật trạng thái hóa đơn và phiếu sửa xe thành công sau khi thanh toán"));
            }
            return BadRequest(ApiResponse<string>.ErrorResponse("Cập nhật trạng thái thất bại. Vui lòng kiểm tra lại hóa đơn."));
        }

    }
}
