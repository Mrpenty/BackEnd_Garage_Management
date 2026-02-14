namespace Garage_Management.Application.DTOs.Vehicles
{
    public class VehicleResponse
    {
        public int VehicleId { get; set; }
        public int CustomerId { get; set; }
        public int BrandId { get; set; }
        public int ModelId { get; set; }
        public string? LicensePlate { get; set; }
        public int? Year { get; set; }
        public string? Vin { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
