using Garage_Management.Base.Common.Enums;

namespace Garage_Management.Application.DTOs.Iventories.StockTransactions
{
    public class StockTransactionResponse
    {
        public int StockTransactionId { get; set; }
        public int SparePartId { get; set; }
        public string? PartCode { get; set; }
        public string PartName { get; set; } = string.Empty;
        public TransactionType TransactionType { get; set; }
        public int QuantityChange { get; set; }
        public decimal UnitPrice { get; set; }
        public int? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public int? JobCardId { get; set; }
        public string? ReceiptCode { get; set; }
        public string? LotNumber { get; set; }
        public string? SerialNumber { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
