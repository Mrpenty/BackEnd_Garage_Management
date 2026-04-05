namespace Garage_Management.Application.DTOs.Invoices
{
    public class BankTransferResponse
    {
        public int InvoiceId { get; set; }
        public decimal Amount { get; set; }

        /// <summary>
        /// URL anh QR code de khach quet thanh toan
        /// </summary>
        public string QrCodeUrl { get; set; } = string.Empty;

        /// <summary>
        /// Thong tin chuyen khoan
        /// </summary>
        public string BankName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string TransferContent { get; set; } = string.Empty;
    }
}
