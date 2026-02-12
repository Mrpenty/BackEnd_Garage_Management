using Garage_Management.Base.Common.Base;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Base.Entities.RepairEstimaties;
using Garage_Management.Base.Entities.Warranties;
using System;
using System.Collections.Generic;

namespace Garage_Management.Base.Entities.Inventories
{
    /// <summary>
    /// Bảng Inventory - Danh mục phụ tùng trong kho của gara
    /// </summary>
    public class Inventory : AuditableEntity
    {
        public int SparePartId { get; set; }

        /// <summary>
        /// Tên phụ tùng
        /// </summary>
        public string PartName { get; set; } = string.Empty;

        /// <summary>
        /// Đơn vị tính (cái, bộ, lít,...)
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// Hãng / nhà sản xuất phụ tùng
        /// </summary>
        public int? SparePartBrandId { get; set; }

        /// <summary>
        /// Thông tin hãng phụ tùng
        /// </summary>
        public SparePartBrand? SparePartBrand { get; set; }

        /// <summary>
        /// Giá nhập gần nhất
        /// </summary>
        public decimal? LastPurchasePrice { get; set; }

        /// <summary>
        /// Model xe tương thích với phụ tùng (model_compatible).
        /// </summary>
        public string? ModelCompatible { get; set; }

        /// <summary>
        /// Hãng xe tương thích (Vehicle_brand) - dạng text mô tả.
        /// </summary>
        public string? VehicleBrand { get; set; }

        /// <summary>
        /// Giá bán đề xuất
        /// </summary>
        public decimal? SellingPrice { get; set; }

        /// <summary>
        /// Cờ đánh dấu phụ tùng còn kinh doanh hay đã ngưng
        /// </summary>
        public bool IsActive { get; set; } = true;


        /// <summary>
        /// Các dòng phụ tùng được dùng trên phiếu sửa chữa
        /// </summary>
        public ICollection<JobCardSparePart> JobCardSpareParts { get; set; } = new List<JobCardSparePart>();

        /// <summary>
        /// Các dòng phụ tùng trong báo giá sửa chữa
        /// </summary>
        public ICollection<RepairEstimateSparePart> RepairEstimateSpareParts { get; set; } = new List<RepairEstimateSparePart>();

        /// <summary>
        /// Các lần xuất kho bảo hành phụ tùng
        /// </summary>
        public ICollection<WarrantySparePart> WarrantySpareParts { get; set; } = new List<WarrantySparePart>();
    }
}


