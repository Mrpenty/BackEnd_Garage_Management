namespace Garage_Management.Application.DTOs.Vehicles
{
    public class VehicleUpdateRequest
    {
        public int? BrandId { get; set; }
        public int? ModelId { get; set; }
        public string? LicensePlate { get; set; }
        public int? Year { get; set; }
        public string? Vin { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
