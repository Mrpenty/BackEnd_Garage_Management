using Garage_Management.Base.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Iventories.StockTransactions
{
    public class StockTransactionCreateRequest
    {
        [Required]
        public int SparePartId { get; set; }

        [Required]
        public TransactionType TransactionType { get; set; }

        [Required]
        public int QuantityChange { get; set; }

        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        public int? SupplierId { get; set; }
        public int? JobCardId { get; set; }

        [MaxLength(50)]
        public string? ReceiptCode { get; set; }

        [MaxLength(100)]
        public string? LotNumber { get; set; }

        [MaxLength(100)]
        public string? SerialNumber { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }
    }
}
