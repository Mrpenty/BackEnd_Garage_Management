namespace Garage_Management.Application.DTOs.Vehicles
{
    public class VehicleCreateRequest
    {
        public int CustomerId { get; set; }
        public int BrandId { get; set; }
        public int ModelId { get; set; }
        public string? LicensePlate { get; set; }
        public int? Year { get; set; }
        public string? Vin { get; set; }
    }
}
